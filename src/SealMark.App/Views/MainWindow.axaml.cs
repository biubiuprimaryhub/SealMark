using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using SealMark.App.Tools;
using SealMark.App.ViewModels;

namespace SealMark.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var topLevel = TopLevel.GetTopLevel(this);
        DataContext = new MainWindowViewModel(topLevel?.StorageProvider!,
            new WindowNotificationManager(topLevel) { MaxItems = 3 }
            );
        DialogHelper.Owner = this;
    }
}