using System;
using System.IO;
using Avalonia.Controls;
using Enviredit.ViewModels;

namespace Enviredit.Views;

public partial class MainWindow : Window
{
    private async void OpenFile()
    {
        Editor.Clear();
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return;
        using StreamReader reader = new StreamReader(vm.CurrentFile);
        string content = await reader.ReadToEndAsync();
        Editor.Document.Text = content;
    }
    private void PopulateFileList()
    {
        FileList.Items.Clear();
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return;
        foreach (var file in Directory.GetFiles(vm.CurrentFolder))
        {
            var name = Path.GetFileName(file);
            FileList.Items.Add(name);
        }
    }
}