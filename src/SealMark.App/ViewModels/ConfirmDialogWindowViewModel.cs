using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace SealMark.App.ViewModels
{
    public partial class ConfirmDialogWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title="提示信息";
        [ObservableProperty]
        private string _message = "确认操作？";
        [ObservableProperty]
        private string _confirmBtnText = "确认";
        [ObservableProperty]
        private string _cancelBtnText = "取消";

        public Action<bool> CloseRequested;
        [RelayCommand]
        private void Confirm()
        {
            CloseRequested?.Invoke(true);
        }
        [RelayCommand]
        private void Cancel()
        {
            CloseRequested?.Invoke(false);
        }
    }
}
