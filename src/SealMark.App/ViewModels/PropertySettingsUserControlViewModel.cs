using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SealMark.App.ViewModels
{
    public partial class PropertySettingsUserControlViewModel : ObservableValidator
    {
        [ObservableProperty]
        private int _watermarkTypeIndex = 0;
        [ObservableProperty]
        private string _watermarkText = string.Empty;
        [ObservableProperty]
        private string _watermarkImgPath=string.Empty;
        [ObservableProperty]
        private Color _markColor = Color.Parse("#80FFFFFF");
        [ObservableProperty]
        private double _markRotation = 0;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "请输入1~1000之间的整数")]
        private decimal? _markTextSize = 30;
        [ObservableProperty]
        private int _markPlacementMode = 0;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "请输入1~1000之间的整数")]
        private int? _xGap = 50;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "请输入1~1000之间的整数")]
        private int? _yGap = 50;
        [ObservableProperty]
        private bool _gapItemVisible = false;
        [ObservableProperty]
        private IImage _watermarkImg;
        [ObservableProperty]
        private bool _watermarkTextItemVisible = true;
        [ObservableProperty]
        private bool _watermarkImgItemVisible = false;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "请输入0.01~50之间的数")]
        private double? _watermarkImgScaleFactor = 1.0;
        [ObservableProperty]
        private double _watermarkImgOpacity = 0;

        public IStorageProvider? StorageProvider { get; set; } = null;
        partial void OnMarkPlacementModeChanged(int value)
        {
            GapItemVisible = value == 1;
        }
        partial void OnWatermarkTypeIndexChanged(int value)
        {
            WatermarkTextItemVisible = value == 0;
            WatermarkImgItemVisible = value == 1;
        }
        [RelayCommand]
        private async Task SelectWatermarkPNGAsync()
        {
            if (StorageProvider == null) return;

            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择水印图片",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("图片文件") { Patterns = new[] { "*.png"} }
                }
            });
            var file= files.FirstOrDefault();
            if (file == null)
                return;
            var path = file.TryGetLocalPath();
            if (!string.IsNullOrEmpty(path))
            {
                WatermarkImgPath = path;
                using (var stream= File.OpenRead(path))
                {
                    if(WatermarkImg!=null)
                    {
                        (WatermarkImg as IDisposable).Dispose();//释放旧资源
                    }
                    WatermarkImg = new Bitmap(stream);
                }
            }
        }
    }
}
