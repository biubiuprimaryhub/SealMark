using Avalonia;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SealMark.App.Models;
using SealMark.App.Tools;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SealMark.App.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// 窗体标题
        /// </summary>
        [ObservableProperty]
        private string _title="铃印-水印工具";
        /// <summary>
        /// 水印参数
        /// </summary>
        [ObservableProperty]
        private PropertySettingsUserControlViewModel _watermarkProperty=new();
        /// <summary>
        /// 待处理的水印图片
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<WatermarkImageItemViewModel> _watermarkImageItemList = new();
        /// <summary>
        /// 是否显示进度
        /// </summary>
        [ObservableProperty]
        private bool _progressShow = false;
        [ObservableProperty]
        private double _progressMaxValue = 100;
        [ObservableProperty]
        private double _progressMinValue = 0;
        [ObservableProperty]
        private double _progressValue = 0;
        [ObservableProperty]
        private string _progressTextTip = "正在处理。。。";

        public int PreviewWidth { get; } = 1000;
        public int PreviewHeight { get; } = 625;

        private readonly IStorageProvider _storageProvider;
        private readonly WindowNotificationManager? _notificationManager;

        public MainWindowViewModel(IStorageProvider storageProvider, WindowNotificationManager notificationManager) :this()
        {
            _storageProvider = storageProvider;
            _notificationManager = notificationManager;
            _notificationManager.Position = NotificationPosition.BottomCenter;
        }
        public MainWindowViewModel()
        {
            
        }
        [RelayCommand]
        private async Task AddImagesAsync()
        {
            if (_storageProvider == null) return;

            var files = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择图片",
                AllowMultiple = true,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("图片文件") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" } }
                }
            });
            ShowProgress(files.Count, 0, "图片加载中...");
            int completed = 0;
            foreach (var file in files)
            {
                var path = file.TryGetLocalPath();
                if (!string.IsNullOrEmpty(path))
                {
                    var item = new WatermarkImageItemViewModel
                    {
                        FilePath = path,
                        FileName = Path.GetFileName(path)
                    };
                    await item.LoadPreviewAsync(); // 立即加载预览图
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        WatermarkImageItemList.Add(item);
                    });
                }
                completed++;
                SetProgressValue(completed);
            }
            CloseProgress();
        }
        [RelayCommand]
        private async Task ClearImages()
        {
            bool confirm = await DialogHelper.ShowConfirmDialog(new ConfirmDialogWindowViewModel()
            {
                Message = "确认清空全部图片？"
            });
            if(confirm)
            {
                foreach (var item in WatermarkImageItemList)
                {
                    (item.PreviewSource as IDisposable).Dispose();//释放图片资源
                }
                WatermarkImageItemList.Clear();
            }
        }
        [RelayCommand]
        private async Task DeleteImage(WatermarkImageItemViewModel item)
        {
            bool confirm = await DialogHelper.ShowConfirmDialog(new ConfirmDialogWindowViewModel()
            {
                Message="确认移除？"
            });
            if(confirm)
            {
                (item.PreviewSource as IDisposable).Dispose();//释放图片资源
                WatermarkImageItemList.Remove(item);
            }
        }
        [RelayCommand]
        private void ChangeTheme(string themeName)
        {
            //设置主题
            Application.Current.RequestedThemeVariant = new ThemeVariant(themeName,ThemeVariant.Light);
        }
        [RelayCommand]
        private async Task ExportImagesAsync()
        {
            if(WatermarkImageItemList.Count==0)
            {
                _notificationManager?.Show(new Notification()
                {
                    Title="操作失败",
                    Message="当前没有可供导出的图片，请先添加图片！",
                    Type=NotificationType.Error
                });
                return;
            }
            if (_storageProvider is null) return;
            var result = await _storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                Title = "选择保存文件夹",
                AllowMultiple = false,
            });
            var folder = result.FirstOrDefault();
            if (folder == null)
                return;
            var folderPath = folder.TryGetLocalPath();
            if(!string.IsNullOrWhiteSpace(folderPath)&&Directory.Exists(folderPath))
            {
                ShowProgress(WatermarkImageItemList.Count, 0,"正在导出...");
                try
                {
                    //设置最大并发导出
                    int maxConcurrency = Math.Min(Environment.ProcessorCount * 2, 8);
                    int completed = 0;
                    await Parallel.ForEachAsync(WatermarkImageItemList.ToList(),
                        new ParallelOptions() { MaxDegreeOfParallelism = maxConcurrency },
                         (item, cancelToken) =>
                        {
                            ExportWithWatermark(item, folderPath);
                            SetProgressValue(Interlocked.Increment(ref completed));
                            return ValueTask.CompletedTask;
                        });
                    SetProgressValue(completed);
                    await Task.Delay(300);
                }
                catch (Exception ex)
                {
                    _notificationManager?.Show(new Notification()
                    {
                        Title = "操作失败",
                        Message = ex.Message,
                        Type = NotificationType.Error
                    });
                    return;
                }
                finally
                {
                    CloseProgress();
                }
            }
            _notificationManager?.Show(new Notification()
            {
                Title = "保存成功",
                Message = "所有图片已成功导出！",
                Type = NotificationType.Success
            });
        }
        private void ExportWithWatermark(WatermarkImageItemViewModel watermarkImageItemViewModel, string outputPath)
        {
            //加载原图
            using var sourceBitmap = new Bitmap(watermarkImageItemViewModel.FilePath);
            var imageSize = sourceBitmap.Size;
            var pixelSize = new PixelSize((int)imageSize.Width, (int)imageSize.Height);
            string extend = Path.GetExtension(watermarkImageItemViewModel.FilePath).ToLower();
            string destinationFilePath = Path.Combine(outputPath
                , $"{Path.GetFileNameWithoutExtension(watermarkImageItemViewModel.FilePath)}_{Guid.NewGuid().ToString("N").Substring(0, 8)}{extend}");

            //创建离屏渲染目标
            using (var renderTarget = new RenderTargetBitmap(pixelSize, new Vector(96, 96)))
            {
                //创建 DrawingContext 并绘制
                using (var context = renderTarget.CreateDrawingContext())
                {
                    //先画原图
                    context.DrawImage(sourceBitmap, new Rect(0, 0, imageSize.Width, imageSize.Height));
                    //再画水印
                    WatermarkHelper.DrawWatermark(context, new WatermarkParameter(
                    WatermarkProperty.WatermarkText,
                    new SolidColorBrush(WatermarkProperty.MarkColor),
                    WatermarkProperty.MarkRotation,
                    (double)WatermarkProperty.MarkTextSize!.Value,
                    WatermarkProperty.MarkPlacementMode, imageSize, imageSize,
                    WatermarkProperty.XGap!.Value,WatermarkProperty.YGap!.Value
                    ));
                }
                
                BitmapEncoderOptions bitmapEncoderOptions = extend switch
                {
                    ".jpg" or ".jpeg" => new JpegBitmapEncoderOptions(),
                    _ => PngBitmapEncoderOptions.Default
                };
                renderTarget.Save(destinationFilePath, bitmapEncoderOptions);
            }
        }
        private void ShowProgress(double maxValue,double minValue,string content)
        {
            ProgressShow = true;
            ProgressMaxValue = maxValue;
            ProgressMinValue = minValue;
            ProgressTextTip = content;
        }
        private void SetProgressValue(double value)
        {
            Dispatcher.UIThread.Post(() =>
            {
                ProgressValue = value;
            });
        }
        private void CloseProgress()
        {
            ProgressShow = false;
            ProgressValue = 0;
        }
    }
}
