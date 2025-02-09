using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Enviredit.Views;

public partial class MainWindow : Window
{
    private void FileList_OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem == null) return;
        CurrentFile = (string)FileList.SelectedItem;
        CurrentFile = CurrentFolder + Path.DirectorySeparatorChar + CurrentFile;
        OpenFile();
    }
}