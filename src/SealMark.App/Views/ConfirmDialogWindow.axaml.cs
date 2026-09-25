using Avalonia.Controls;
using SealMark.App.ViewModels;

namespace SealMark.App.Views
{
    public partial class ConfirmDialogWindow : Window
    {
        public ConfirmDialogWindow(ConfirmDialogWindowViewModel confirmDialogWindowViewModel)
        {
            InitializeComponent();
            DataContext = confirmDialogWindowViewModel;
            confirmDialogWindowViewModel.CloseRequested += (r) =>
            {
                Close(r);
            };
        }
    }
}