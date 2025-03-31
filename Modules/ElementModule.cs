using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Enviredit.Views;

public partial class MainWindow : Window
{
    // Functions for the file list
    private void FileList_OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem == null) return;
        LocalSetCurrentFile(LocalGetCurrentFolder() + Path.DirectorySeparatorChar + (string)FileList.SelectedItem);
        OpenFile();
    }
    private void LoadFileContext_OnClick(object? sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem == null) return;
        LocalSetCurrentFile(LocalGetCurrentFolder() + Path.DirectorySeparatorChar + (string)FileList.SelectedItem);
        OpenFile();
    }
    private void DeleteFileContext_OnClick(object? sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem == null) return;
        LocalSetDeletionFile(LocalGetCurrentFolder() + Path.DirectorySeparatorChar + (string)FileList.SelectedItem);
        DeleteFile();
        PopulateFileList();
    }
}