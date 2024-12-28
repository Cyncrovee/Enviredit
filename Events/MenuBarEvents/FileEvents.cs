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
        if (FilePath == String.Empty)
        {
            await SaveFileAs();
            RefreshList();
        }
        else
        {
            await SaveFile();
            RefreshList();
        }
    }
    private async void SaveAsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await SaveFileAs();
        RefreshList();
    }
    private void OpenContainingFolder_OnClick(object? sender, RoutedEventArgs e)
    {
        if (FilePath == String.Empty) return;
        FileInfo fileInfo = new FileInfo(FilePath);
        var directoryPath = fileInfo.Directory;
        FolderPath = directoryPath.FullName;
        RefreshList();
    }
    private void LastFolderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var jsonString = File.ReadAllText(SettingsFile);
        var userSettings = JsonSerializer.Deserialize<UserSettings>(jsonString);
        if (userSettings.LastUsedFolder == null) return;
        FolderPath = userSettings.LastUsedFolder;
        RefreshList();
    }
    private void LastFileButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Text = String.Empty;
        Editor.Clear();
        var jsonString = File.ReadAllText(SettingsFile);
        var userSettings = JsonSerializer.Deserialize<UserSettings>(jsonString);
        if (userSettings.LastUsedFile == null) return;
        FilePath = userSettings.LastUsedFile;
        LoadFile();
        RefreshFileInformation();
        SaveSettings();
        
    }
    private void ExitFile_OnClick(object? sender, RoutedEventArgs e)
    {
        FilePath = String.Empty;
        Encoding = String.Empty;
        Extension = String.Empty;
        Language = String.Empty;
        
        Editor.TextArea.Caret.Column = 1;
        Editor.TextArea.Caret.Line = 1;
        Editor.Clear();
        RefreshFileInformation();
    }
    private void ExitFolder_OnClick(object? sender, RoutedEventArgs e)
    {
        FolderPath = String.Empty;
        FileList.Items.Clear();
    }
    private void Exit(object? sender, RoutedEventArgs e)
    {
        SaveSettings();
        Close();
    }
}