using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Enviredit;

public partial class MainWindow : Window
{
    private void GridLinesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        MainGrid.ShowGridLines = !MainGrid.ShowGridLines;
        SaveSettings();
    }
}