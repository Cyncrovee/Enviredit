using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Enviredit;

public partial class MainWindow : Window
{
    private async void About_OnClick(object? sender, RoutedEventArgs e)
    {
        var ownerWindow = this;
        var dialogWindow = new AboutWindow();
        dialogWindow.RequestedThemeVariant = RequestedThemeVariant;
        await dialogWindow.ShowDialog(ownerWindow);
    }
}