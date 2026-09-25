using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace SealMark.App.ViewModels
{
    public partial class WatermarkImageItemViewModel :ObservableObject
    {
        public string FilePath { get; set; } = "";
        public string FileName { get; set; } = "";

        private const double MAXHEIGHT = 600;
        private const double MAXWIDTH = 1000;

        [ObservableProperty]
        private IImage? _previewSource;

        public double OriginalWidth { get; private set; }
        public double OriginalHeight { get; private set; }
        public double ShowBoxWidth { get; private set; } = MAXWIDTH;
        public double ShowBoxHeight {  get; private set; }= MAXHEIGHT;

        public async Task LoadPreviewAsync()
        {
            if (!File.Exists(FilePath) || PreviewSource != null) return;

            await Task.Run(() =>
            {
                using (var stream = File.OpenRead(FilePath))
                {
                    double aspectRatio = 1;
                    using (var original = new Bitmap(stream))
                    {
                        // 保存原图尺寸
                        OriginalWidth = original.Size.Width;
                        OriginalHeight = original.Size.Height;
                        aspectRatio= original.Size.AspectRatio;
                    }
                    stream.Seek(0, SeekOrigin.Begin); // 重置流位置

                    //横向图，按最大宽度限制
                    if(OriginalWidth > OriginalHeight)
                    {
                        if(OriginalWidth > MAXWIDTH)
                        {
                            PreviewSource = Bitmap.DecodeToWidth(stream, (int)MAXWIDTH);
                        }
                        else
                        {
                            PreviewSource = new Bitmap(stream);
                        }
                    }
                    else//竖图，按最大高度限制
                    {
                        if(OriginalHeight > MAXHEIGHT)
                        {
                            PreviewSource = Bitmap.DecodeToHeight(stream, (int)MAXHEIGHT);
                        }
                        else
                        {
                            PreviewSource = new Bitmap(stream);
                        }
                    }
                    // 从缩放后的图读取实际尺寸
                    ShowBoxWidth = PreviewSource.Size.Width;
                    ShowBoxHeight = PreviewSource.Size.Height + 31;
                }
            });
        }
    }
}
