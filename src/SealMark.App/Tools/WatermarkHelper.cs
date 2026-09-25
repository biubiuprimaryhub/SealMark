using Avalonia;
using Avalonia.Media;
using SealMark.App.Models;
using System;
using System.Globalization;

namespace SealMark.App.Tools
{
    public class WatermarkHelper
    {
        // 内部常量：角落边距（像素，基于原图尺寸）
        private const double CornerMargin = 10;
        /// <summary>
        /// 画水印
        /// </summary>
        /// <param name="context"></param>
        /// <param name="watermarkParameter">水印参数</param>
        public static void DrawWatermark(DrawingContext context, WatermarkParameter watermarkParameter)
        {
            if (string.IsNullOrEmpty(watermarkParameter.WatermarkText) || watermarkParameter.MarkColor == null) return;
            if (watermarkParameter.ImageSize.Width <= 0 || watermarkParameter.ImageSize.Height <= 0) return;

            // 计算预览缩放比例
            double previewWidth = 0.0, previewHeight=0.0;
            if (watermarkParameter.PreviewSize==default)
            {
                previewWidth = watermarkParameter.ImageSize.Width;
                previewHeight= watermarkParameter.ImageSize.Height;
            }
            else
            {
                previewWidth=watermarkParameter.PreviewSize.Width;
                previewHeight = watermarkParameter.PreviewSize.Height;
            }
            double scaleX = previewWidth / watermarkParameter.ImageSize.Width;
            double scaleY = previewHeight / watermarkParameter.ImageSize.Height;
            double minScale = Math.Min(scaleX, scaleY);
            var typeface = new Typeface(Application.Current?.Resources["MyCustomFontFamily"] as FontFamily);
            var formattedText = new FormattedText(
                watermarkParameter.WatermarkText,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                watermarkParameter.MarkTextSize * minScale,// 字体按缩放比例调整
                watermarkParameter.MarkColor
            );

            var textWidth = formattedText.Width;
            var textHeight = formattedText.Height;


            // "半宽半高"，循环里只用来算中心点
            double halfW = textWidth / 2;
            double halfH = textHeight / 2;


            // 根据位置模式计算水印位置
            var positions = CalculatePositions(watermarkParameter.MarkPlacementMode
                , watermarkParameter.ImageSize.Width
                , watermarkParameter.ImageSize.Height
                , textWidth / scaleX
                , textHeight / scaleY
                , scaleX, scaleY
                ,watermarkParameter.XGap
                ,watermarkParameter.YGap);
            using (context.PushClip(new Rect(0, 0, previewWidth, previewHeight)))
            {

                foreach (var pos in positions)
                {
                    double x = pos.X * scaleX;
                    double y = pos.Y * scaleY;

                    // 每个水印的中心点
                    double centerX = x + halfW;
                    double centerY = y + halfH;

                    var transform = new RotateTransform(watermarkParameter.MarkRotation, centerX, centerY).Value;

                    using (context.PushTransform(transform))
                    {
                        context.DrawText(formattedText, new Point(x, y));
                    }
                }
            }
        }
        public static Point[] CalculatePositions(int mode, double imageWidth, double imageHeight, double textWidth, double textHeight, double scaleX, double scaleY,int xGap,int yGap)
        {
            switch (mode)
            {
                case 0: // 居中
                    return
                    [
                        new Point((imageWidth - textWidth) / 2, (imageHeight - textHeight) / 2)
                    ];

                case 1: // 平铺全屏
                    // 平铺间距 = 文字尺寸 + 间隙（基于原图尺寸计算）
                    double tileGapX = (textWidth  + xGap);
                    double tileGapY = (textHeight  + yGap);
                    var cols = (int)Math.Ceiling(imageWidth / tileGapX) + 2;
                    var rows = (int)Math.Ceiling(imageHeight / tileGapY) + 2;
                    var positions = new Point[cols * rows];
                    int idx = 0;
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            // 直接从 (0, 0) 开始平铺
                            double x = j * tileGapX;
                            double y = i * tileGapY;

                            // 超出图片范围的坐标直接跳过，不加入列表
                            if (x >= imageWidth || y >= imageHeight)
                                continue;

                            positions[idx++] = new Point(x , y );
                        }
                    }
                    var results = new Point[idx];
                    Array.Copy(positions, results, idx);
                    return results;

                case 2: // 左上角
                    return [new Point(CornerMargin, CornerMargin)];

                case 3: // 右上角
                    return [new Point(imageWidth - textWidth - CornerMargin, CornerMargin)];

                case 4: // 右下角
                    return [new Point(imageWidth - textWidth - CornerMargin, imageHeight - textHeight - CornerMargin)];

                case 5: // 左下角
                    return [new Point(CornerMargin, imageHeight - textHeight - CornerMargin)];

                default:
                    return Array.Empty<Point>();
            }
        }
    }
}
