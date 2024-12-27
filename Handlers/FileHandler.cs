using System;
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
    
    private async Task SaveFile()
    {
        if (FilePath == String.Empty) return;
        await using (var writer = new StreamWriter(FilePath))
        {
            await writer.WriteAsync(Editor.Text);
            writer.Close();
            Console.WriteLine("File saved");
        }
    }
    private async Task SaveFileAs()
    {
        var topLevel = GetTopLevel(this);

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save File"
        });

        if (file is not null)
        {
            FilePath = file.Path.LocalPath;
            await using (var writer = new StreamWriter(FilePath))
            {
                await writer.WriteLineAsync(Editor.Text);
                writer.Close();
            }
            RefreshFileInformation();
        }
        else
        {
            Console.WriteLine("No file created");
        }
    }

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