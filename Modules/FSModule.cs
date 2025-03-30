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
        using StreamReader reader = new StreamReader(LocalGetCurrentFile());
        string content = await reader.ReadToEndAsync();
        Editor.Document.Text = content;
    }
    private void PopulateFileList()
    {
        FileList.Items.Clear();
        foreach (var file in Directory.GetFiles(LocalGetCurrentFolder()))
        {
            var name = Path.GetFileName(file);
            FileList.Items.Add(name);
        }
    }
}