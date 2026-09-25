using Avalonia.Controls;
using SealMark.App.ViewModels;
using SealMark.App.Views;
using System.Threading.Tasks;

namespace SealMark.App.Tools
{
    public class DialogHelper
    {
        public static Window Owner;
        /// <summary>
        /// 确认弹框
        /// </summary>
        /// <param name="confirmDialogWindowViewModel"></param>
        /// <returns></returns>
        public static Task<bool> ShowConfirmDialog(ConfirmDialogWindowViewModel confirmDialogWindowViewModel)
        {
            ConfirmDialogWindow dialogWindow = new ConfirmDialogWindow(confirmDialogWindowViewModel);
            return dialogWindow.ShowDialog<bool>(Owner);
        }
    }
}
