using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using SealMark.App.Models;
using SealMark.App.Tools;

namespace SealMark.App.Controls
{
    public class WatermarkOverlayControl : Control
    {
        // 内部常量：平铺间距（像素，基于原图尺寸）
        private const double TileGap = 200;
        // 内部常量：角落边距（像素，基于原图尺寸）
        private const double CornerMargin = 20;
        /// <summary>
        /// 静态构造函数（整个程序只执行一次，在第一个实例创建之前，通常用作初始化类级别的静态数据），注册影响渲染的属性
        /// </summary>
        static WatermarkOverlayControl()
        {
            AffectsRender<WatermarkOverlayControl>(
                WatermarkTextProperty,
                FontSizeProperty,
                TextColorProperty,
                AngleProperty,
                PlacementModeProperty,
                TileXGapProperty,
                TileYGapProperty
                );
        }

        public static readonly StyledProperty<string> WatermarkTextProperty =
            AvaloniaProperty.Register<WatermarkOverlayControl, string>(nameof(WatermarkText));

        public string WatermarkText
        {
            get => GetValue(WatermarkTextProperty);
            set => SetValue(WatermarkTextProperty, value);
        }

        public static readonly StyledProperty<double> FontSizeProperty =
            AvaloniaProperty.Register<WatermarkOverlayControl, double>(nameof(FontSize), 24);

        public double FontSize
        {
            get => GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly StyledProperty<IBrush?> TextColorProperty =
            AvaloniaProperty.Register<WatermarkOverlayControl, IBrush?>(nameof(TextColor));

        public IBrush? TextColor
        {
            get => GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly StyledProperty<double> AngleProperty =
            AvaloniaProperty.Register<WatermarkOverlayControl, double>(nameof(Angle), -30);

        public double Angle
        {
            get => GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly StyledProperty<int> PlacementModeProperty =
            AvaloniaProperty.Register<WatermarkOverlayControl, int>(nameof(PlacementMode), 0);

        public int PlacementMode
        {
            get => GetValue(PlacementModeProperty);
            set => SetValue(PlacementModeProperty, value);
        }

        public static readonly StyledProperty<int> TileXGapProperty =
            AvaloniaProperty.Register<WatermarkOverlayControl, int>(nameof(TileXGap), 50);

        public int TileXGap
        {
            get => GetValue(TileXGapProperty);
            set => SetValue(TileXGapProperty, value);
        }

        public static readonly StyledProperty<int> TileYGapProperty =
            AvaloniaProperty.Register<WatermarkOverlayControl, int>(nameof(TileYGap), 50);

        public int TileYGap
        {
            get => GetValue(TileYGapProperty);
            set => SetValue(TileYGapProperty, value);
        }

        // 原图尺寸（用于计算位置）
        public static readonly StyledProperty<double> OriginalImageWidthProperty =
            AvaloniaProperty.Register<WatermarkOverlayControl, double>(nameof(OriginalImageWidth));

        public double OriginalImageWidth
        {
            get => GetValue(OriginalImageWidthProperty);
            set => SetValue(OriginalImageWidthProperty, value);
        }

        public static readonly StyledProperty<double> OriginalImageHeightProperty =
            AvaloniaProperty.Register<WatermarkOverlayControl, double>(nameof(OriginalImageHeight));

        public double OriginalImageHeight
        {
            get => GetValue(OriginalImageHeightProperty);
            set => SetValue(OriginalImageHeightProperty, value);
        }

        // 实际显示尺寸
        public static readonly StyledProperty<double> ActualImageWidthProperty =
            AvaloniaProperty.Register<WatermarkOverlayControl, double>(nameof(ActualImageWidth));

        public double ActualImageWidth
        {
            get => GetValue(ActualImageWidthProperty);
            set => SetValue(ActualImageWidthProperty, value);
        }
        public static readonly StyledProperty<double> ActualImageHeightProperty =
            AvaloniaProperty.Register<WatermarkOverlayControl, double>(nameof(ActualImageHeight));

        public double ActualImageHeight
        {
            get => GetValue(ActualImageHeightProperty);
            set => SetValue(ActualImageHeightProperty, value);
        }


        public override void Render(DrawingContext context)
        {
            base.Render(context);
            if (string.IsNullOrEmpty(WatermarkText) || TextColor == null) return;
            if (OriginalImageWidth <= 0 || OriginalImageHeight <= 0) return;

            WatermarkHelper.DrawWatermark(context,
                new WatermarkParameter(
                    WatermarkText,
                    TextColor,
                    Angle,
                    FontSize,
                    PlacementMode,
                    new Size(OriginalImageWidth, OriginalImageHeight),
                    new Size(ActualImageWidth, ActualImageHeight),
                    TileXGap,
                    TileYGap
                    ));
        }
    }
}
