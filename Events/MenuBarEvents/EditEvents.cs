using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Enviredit;

public partial class MainWindow : Window
{
    private void UndoButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Undo();
    }
    private void RedoButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Redo();
    }
    private void SelectAll_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.SelectAll();
    }
    private void Cut(object? sender, RoutedEventArgs e)
    {
        Editor.Cut();
    }
    private void Copy(object? sender, RoutedEventArgs e)
    {
        Editor.Copy();
    }
    private void Paste(object? sender, RoutedEventArgs e)
    {
        Editor.Paste();
    }
    private void Delete(object? sender, RoutedEventArgs e)
    {
        Editor.Delete();
    }
    private void CopyFolderPathButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Clipboard == null || FolderPath == String.Empty) return;
        Clipboard.SetTextAsync(FolderPath);
    }
    private void CopyFilePathButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Clipboard == null || FilePath == String.Empty) return;
        Clipboard.SetTextAsync(FilePath);
    }
    private void Clear(object? sender, RoutedEventArgs e)
    {
        Editor.Text= String.Empty;
        Editor.Clear();
    }
    private void OpenFind(object? sender, RoutedEventArgs e)
    {
        Editor.SearchPanel.Close();
        Editor.SearchPanel.Open();
        Editor.SearchPanel.IsReplaceMode = false;
    }
    private void OpenFindReplace(object? sender, RoutedEventArgs e)
    {
        Editor.SearchPanel.Close();
        Editor.SearchPanel.Open();
        Editor.SearchPanel.IsReplaceMode = true;
    }
    private void ConvertToUppercase_OnClick(object? sender, RoutedEventArgs e)
    {
        var convertedText = Editor.SelectedText.ToUpper();
        Editor.SelectedText = convertedText;
    }
    private void ConvertToLowercase_OnClick(object? sender, RoutedEventArgs e)
    {
        var convertedText = Editor.SelectedText.ToLower();
        Editor.SelectedText = convertedText;
    }
    private void ToggleWordWrapButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.WordWrap = !Editor.WordWrap;
    }
    private void ToggleRectangularSelectionButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.EnableRectangularSelection = !Editor.Options.EnableRectangularSelection;
        SaveSettings();
    }
}