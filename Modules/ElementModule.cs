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
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return;
        vm.CurrentFile = (string)FileList.SelectedItem;
        vm.CurrentFile = vm.CurrentFolder + Path.DirectorySeparatorChar + vm.CurrentFile;
        OpenFile();
    }
}