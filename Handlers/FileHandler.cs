using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace Enviredit.Handlers;

public class FileHandler
{
    private static readonly FileStreamOptions FileOptions = new()
    {
        Mode = FileMode.Open,
        Access = FileAccess.Read,
    };
    public async Task OpenFileDialog(MainWindow window)
    {
        var topLevel = TopLevel.GetTopLevel(window);
        var file = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select File",
            AllowMultiple = false
        });

        window.FilePath = file.First().Path.LocalPath;
    }
    public void LoadFile(MainWindow window)
    {
        window.Editor.Clear();
        using (var reader = new StreamReader(window.FilePath, FileOptions))
        {
            window.Editor.Document.Text = reader.ReadToEnd();
            reader.Peek();
            window.Encoding = reader.CurrentEncoding.EncodingName;
            reader.Close();
        }
    }
}