using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Enviredit.ViewModels;

namespace Enviredit.Views;

public partial class MainWindow : Window
{
    private void FileList_OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem == null) return;
        LocalSetCurrentFile((string)FileList.SelectedItem);
        LocalSetCurrentFile(LocalGetCurrentFolder() + Path.DirectorySeparatorChar + LocalGetCurrentFile());
        OpenFile();
    }
}