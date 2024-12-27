using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace Enviredit.Handlers;

public class FolderHandler
{
    public async Task OpenFolderDialog(MainWindow window)
    {
        var topLevel = TopLevel.GetTopLevel(window);
        var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Open Folder",
            AllowMultiple = false
        });

        if (folder != null)
        {
            window.FolderPath = folder[0].Path.LocalPath;
            Console.WriteLine(window.FolderPath);
        }
        else
        {
            Console.WriteLine("Failed to open folder");
        }
    }
}