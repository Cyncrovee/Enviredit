using System.IO;
using Avalonia.Controls;

namespace Enviredit.Views;

public partial class MainWindow : Window
{
    private async void OpenFile()
    {
        // Read text from a file
        // Then set the editors text contents to the file contents
        Editor.Clear();
        using StreamReader reader = new StreamReader(LocalGetCurrentFile());
        string content = await reader.ReadToEndAsync();
        Editor.Document.Text = content;
    }
    private void PopulateFileList()
    {
        // Get the names of the files from the current folder
        // Then add them to the file list
        FileList.Items.Clear();
        foreach (var file in Directory.GetFiles(LocalGetCurrentFolder()))
        {
            var name = Path.GetFileName(file);
            FileList.Items.Add(name);
        }
    }
}