using Avalonia;
using Avalonia.Media;

namespace SealMark.App.Models
{
    public record WatermarkParameter(string WatermarkText, IBrush? MarkColor,double MarkRotation
        ,double MarkTextSize,int MarkPlacementMode,Size ImageSize,Size PreviewSize=default
        ,int XGap=50,int YGap=50)
    {
    }
}
