using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Styling;

namespace Enviredit.Views;

public partial class MainWindow : Window
{
    //////////
    //
    // FILE
    //
    //////////
    private async void Open_OnClick(object? sender, RoutedEventArgs e)
    {
        var file = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select File to Open",
            AllowMultiple = false,
        });
        if (file.Count == 0) return;
        var selectedFile = file[0].TryGetLocalPath();
        if (selectedFile == null) return;
        LocalSetCurrentFile(selectedFile);
        OpenFile();
    }
    private async void OpenFolder_OnClick(object? sender, RoutedEventArgs e)
    {
        var folder = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Select File to Open",
            AllowMultiple = false
        }
        );
        try
        {
            if (folder[0].TryGetLocalPath() == null) return;
            LocalSetCurrentFolder(folder[0].TryGetLocalPath());
            PopulateFileList();
        }
        catch (Exception exception)
        {
            Console.WriteLine("OpenFolder_OnClick error: " + exception);
        }
    }
    private void Save_OnClick(object? sender, RoutedEventArgs e)
    {
        SaveFile();
        PopulateFileList();
    }
    private async void SaveAs_OnClick(object? sender, RoutedEventArgs e)
    {
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            Title = "Save File As...",
        });
        var selectedFile = file?.TryGetLocalPath();
        if (selectedFile == null) return;
        LocalSetCurrentFile(selectedFile);
        SaveFile();
        PopulateFileList();
    }


    //////////
    //
    // EDIT
    //
    //////////
    // Undo/Redo
    private void Undo_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Undo();
    }
    private void Redo_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Redo();
    }
    // Cut, Copy and Paste
    private void CutText_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Cut();
    }
    private void CopyText_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Copy();
    }
    private void PasteText_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Paste();
    }
    // Delete/Select All Text
    private void DeleteText_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Delete();
    }
    private void SelectAll_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.SelectAll();
    }
    // Open/Close Find/Replace Panel
    private void Find_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.SearchPanel.Open();
        Editor.SearchPanel.IsReplaceMode = false;
    }
    private void FindReplace_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.SearchPanel.Open();
        Editor.SearchPanel.IsReplaceMode = true;
    }
    private void CloseFindPanel_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.SearchPanel.Close();
    }
    // Copy file/folder path to the clipboard (unless clipboard is null)
    private async void CopyFile_OnClick(object? sender, RoutedEventArgs e)
    {
        if (LocalGetCurrentFile() == string.Empty) return;
        if (Clipboard == null) return;
        await Clipboard.SetTextAsync(LocalGetCurrentFile());
    }
    private async void CopyFolder_OnClick(object? sender, RoutedEventArgs e)
    {
        if (LocalGetCurrentFolder() == string.Empty) return;
        if (Clipboard == null) return;
        await Clipboard.SetTextAsync(LocalGetCurrentFolder());
    }
    // Change selected text to uppercase/lowercase
    private void ToUpperCase_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.SelectedText = Editor.SelectedText.ToUpper();
    }
    private void ToLowerCase_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.SelectedText = Editor.SelectedText.ToLower();
    }


    //////////
    //
    // VIEW
    //
    //////////
    private void HighlightLine_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.HighlightCurrentLine = !Editor.Options.HighlightCurrentLine;
        CreateSettingsFile();
        SaveSettings();
    }
    private void ShowSpaces_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowSpaces = !Editor.Options.ShowSpaces;
        CreateSettingsFile();
        SaveSettings();
    }
    private void ShowTabs_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowTabs = !Editor.Options.ShowTabs;
        CreateSettingsFile();
        SaveSettings();
    }
    private void ConvertTabs_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ConvertTabsToSpaces = !Editor.Options.ConvertTabsToSpaces;
        CreateSettingsFile();
        SaveSettings();
    }
    private void ThemeOptions_SelectionChanged(object? sender, RoutedEventArgs e)
    {
        if (ThemeOptions.SelectedItem == null) return;
        switch (ThemeOptions.SelectedIndex)
        {
            case 0:
                this.RequestedThemeVariant = ThemeVariant.Default;
                break;
            case 1:
                this.RequestedThemeVariant = ThemeVariant.Light;
                break;
            case 2:
                this.RequestedThemeVariant = ThemeVariant.Dark;
                break;
        }
        CreateSettingsFile();
        SaveSettings();
    }
    private void IndentOptions_SelectionChanged(object? sender, RoutedEventArgs e)
    {
        Editor.Options.IndentationSize = IndentOptions.SelectedIndex;
        CreateSettingsFile();
        SaveSettings();
    }
    private void VBarOptions_SelectionChanged(object? sender, RoutedEventArgs e)
    {
        if (VBarOptions.SelectedItem == null) return;
        Editor.VerticalScrollBarVisibility = (ScrollBarVisibility)VBarOptions.SelectedItem;
        CreateSettingsFile();
        SaveSettings();
    }
    private void HBarOptions_SelectionChanged(object? sender, RoutedEventArgs e)
    {
        if (VBarOptions.SelectedItem == null) return;
        Editor.VerticalScrollBarVisibility = (ScrollBarVisibility)VBarOptions.SelectedItem;
        CreateSettingsFile();
        SaveSettings();
    }
    private void ShowGridLines_OnClick(object? sender, RoutedEventArgs e)
    {
        MainGrid.ShowGridLines = !MainGrid.ShowGridLines;
        CreateSettingsFile();
        SaveSettings();
    }
    private void ManualLoadSettings_OnClick(object? sender, RoutedEventArgs e)
    {
        CreateSettingsFile();
        LoadSettings();
    }
}