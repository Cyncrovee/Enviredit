using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace Enviredit;

public partial class MainWindow : Window
{
    private async Task OpenFolderDialog()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Open Folder",
            AllowMultiple = false
        });

        if (folder != null)
        {
            FolderPath = folder[0].Path.LocalPath;
            Console.WriteLine(FolderPath);
        }
        else
        {
            Console.WriteLine("Failed to open folder");
        }
    }
}