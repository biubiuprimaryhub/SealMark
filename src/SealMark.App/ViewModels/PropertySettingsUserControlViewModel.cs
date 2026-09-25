using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SealMark.App.ViewModels
{
    public partial class PropertySettingsUserControlViewModel : ObservableValidator
    {
        [ObservableProperty]
        private string _watermarkText = string.Empty;
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
        partial void OnMarkPlacementModeChanged(int value)
        {
            GapItemVisible = value == 1;
        }
    }
}
