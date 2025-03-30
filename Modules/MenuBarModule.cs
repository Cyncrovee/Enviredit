using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Enviredit.ViewModels;

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
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return;
        var file = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select File to Open",
            AllowMultiple = false,
        });
        if (file.Count == 0) return;
        var selectedFile = file[0].TryGetLocalPath();
        if (selectedFile == null) return;
        vm.CurrentFile = selectedFile;
        OpenFile();
    }
    private async void OpenFolder_OnClick(object? sender, RoutedEventArgs e)
    {
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return;
        var folder = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Select File to Open",
            AllowMultiple = false
        }
        );
        if (folder.Count == 0) return;
        if (folder[0] == null) return;
        vm.CurrentFolder = folder[0].TryGetLocalPath();
        Console.WriteLine(vm.CurrentFolder);
        PopulateFileList();
    }
    private void Save_OnClick(object? sender, RoutedEventArgs e)
    {
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return;
        if (vm.CurrentFile == string.Empty) return;
        using StreamWriter writer = new StreamWriter(vm.CurrentFile);
        foreach (var content in Editor.Document.Text)
        {
            writer.WriteAsync(content);
        }
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
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return;
        if (vm.CurrentFile == string.Empty) return;
        if (Clipboard == null) return;
        await Clipboard.SetTextAsync(vm.CurrentFile);
    }
    private async void CopyFolder_OnClick(object? sender, RoutedEventArgs e)
    {
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return;
        if (vm.CurrentFolder == string.Empty) return;
        if (Clipboard == null) return;
        await Clipboard.SetTextAsync(vm.CurrentFolder);
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
    }
    private void ShowSpaces_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowSpaces = !Editor.Options.ShowSpaces;
    }
    private void ShowTabs_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowTabs = !Editor.Options.ShowTabs;
    }
    private void ConvertTabs_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ConvertTabsToSpaces = !Editor.Options.ConvertTabsToSpaces;
    }
    private void VBarOptions_SelectionChanged(object? sender, RoutedEventArgs e)
    {
        if (VBarOptions.SelectedItem == null) return;
        Editor.VerticalScrollBarVisibility = (ScrollBarVisibility)VBarOptions.SelectedItem;
    }
    private void HBarOptions_SelectionChanged(object? sender, RoutedEventArgs e)
    {
        if (VBarOptions.SelectedItem == null) return;
        Editor.VerticalScrollBarVisibility = (ScrollBarVisibility)VBarOptions.SelectedItem;
    }
    private void ShowGridLines_OnClick(object? sender, RoutedEventArgs e)
    {
        MainGrid.ShowGridLines = !MainGrid.ShowGridLines;
    }
}