using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace Enviredit;

public partial class MainWindow : Window
{
    private static readonly FileStreamOptions FileOptions = new()
    {
        Mode = FileMode.Open,
        Access = FileAccess.Read,
    };
    private async Task OpenFileDialog()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        try
        {
            var file = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select File",
                AllowMultiple = false
            });

            if (file.First() == null) return;
            FilePath = file.First().Path.LocalPath;
        }
        catch (OperationCanceledException)
        {
            // Pass
        }
    }
    private void LoadFile()
    {
        Editor.Clear();
        using (var reader = new StreamReader(FilePath, FileOptions))
        {
            Editor.Document.Text = reader.ReadToEnd();
            reader.Peek();
            Encoding = reader.CurrentEncoding.EncodingName;
            reader.Close();
        }
    }
}