using System;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Enviredit;

public partial class MainWindow : Window
{
    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (FilePath == string.Empty) return;
        await using (var writer = new StreamWriter(FilePath))
        {
            await writer.WriteAsync(Editor.Text);
            writer.Close();
            Console.WriteLine("File saved");
        }
    }
    private async void SaveAsButton_OnClick(object? sender, RoutedEventArgs e)
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
            _mainRefreshHandler.RefreshFileInformation(this);
        }
        else
        {
            Console.WriteLine("No file created");
        }
    }
    private void OpenContainingFolder_OnClick(object? sender, RoutedEventArgs e)
    {
        if (FilePath == string.Empty) return;
        FileInfo fileInfo = new FileInfo(FilePath);
        var directoryPath = fileInfo.Directory;
        FolderPath = directoryPath.FullName;
        _mainRefreshHandler.RefreshList(this);
    }
    private void LastFolderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var jsonString = File.ReadAllText(SettingsFile);
        var userSettings = JsonSerializer.Deserialize<UserSettings>(jsonString);
        if (userSettings.LastUsedFolder == null) return;
        FolderPath = userSettings.LastUsedFolder;
        _mainRefreshHandler.RefreshList(this);
    }
    private void LastFileButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Text = string.Empty;
        Editor.Clear();
        var jsonString = File.ReadAllText(SettingsFile);
        var userSettings = JsonSerializer.Deserialize<UserSettings>(jsonString);
        if (userSettings.LastUsedFile == null) return;
        FilePath = userSettings.LastUsedFile;
        _fileHandler.LoadFile(this);
        _mainRefreshHandler.RefreshFileInformation(this);
        MainSettingsHandler.SaveSettings(this);
        
    }
    private void ExitFileFolder_OnClick(object? sender, RoutedEventArgs e)
    {
        FilePath = string.Empty;
        FolderPath = string.Empty;

        Encoding = String.Empty;
        Extension = String.Empty;
        Language = String.Empty;

        FileList.Items.Clear();
        Editor.TextArea.Caret.Column = 1;
        Editor.TextArea.Caret.Line = 1;
        Editor.Clear();
    }
    private void Exit(object? sender, RoutedEventArgs e)
    {
        MainSettingsHandler.SaveSettings(this);;
        this.Close();
    }
}