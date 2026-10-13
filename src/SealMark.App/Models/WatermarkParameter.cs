using Avalonia;
using Avalonia.Media;

namespace SealMark.App.Models
{
    /// <summary>
    /// 水印参数
    /// </summary>
    /// <param name="WatermarkTypeIndex">水印内容类型</param>
    /// <param name="WatermarkText">水印文字</param>
    /// <param name="WatermarkImg">水印图片</param>
    /// <param name="MarkColor">文字颜色</param>
    /// <param name="MarkRotation">水印旋转角度</param>
    /// <param name="MarkTextSize">文字大小</param>
    /// <param name="MarkPlacementMode">水印位置模式</param>
    /// <param name="ImageSize">原始图片尺寸</param>
    /// <param name="PreviewSize">预览图面尺寸</param>
    /// <param name="XGap">水平间距</param>
    /// <param name="YGap">垂直间距</param>
    /// <param name="WatermarkImgScaleFactor">水印图片缩放倍数</param>
    /// <param name="WatermarkImgOpacity">水印图片透明度</param>
    public record WatermarkParameter(int WatermarkTypeIndex,string? WatermarkText,IImage? WatermarkImg, IBrush? MarkColor,double MarkRotation
        ,double MarkTextSize,int MarkPlacementMode,Size ImageSize,Size PreviewSize=default
        ,int XGap=50,int YGap=50,double WatermarkImgScaleFactor=1.0,double WatermarkImgOpacity=0)
    {
    }
}
