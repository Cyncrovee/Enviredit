using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Enviredit;

public partial class MainWindow : Window
{
        private void FullscreenToggleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        switch (WindowState)
        {
            case WindowState.Normal:
                WindowState = WindowState.FullScreen;
                FullscreenToggleButton.IsChecked = true;
                break;
            case WindowState.FullScreen:
                WindowState = WindowState.Normal;
                FullscreenToggleButton.IsChecked = false;
                break;
        }
    }
    private void ToggleTopmostButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Topmost = !Topmost;
    }
    private void EditorViewButton_OnClick(object? sender, RoutedEventArgs e)
    {
        switch (IsEditorView)
        {
            case true:
                FolderPathBlock.IsVisible = true;
                FilePathBlock.IsVisible = true;
                OpenFolderButton.IsVisible = true;
                OpenFileButton.IsVisible = true;
                SettingsButton.IsVisible = true;
                FileList.IsVisible = true;
                ListViewButton.IsVisible = true;
                IsEditorView = false;
                Editor.SetValue(Grid.ColumnProperty, 0);
                Editor.SetValue(Grid.RowProperty, 4);
                Editor.SetValue(Grid.ColumnSpanProperty, 1);
                break;
            case false:
                FolderPathBlock.IsVisible = false;
                FilePathBlock.IsVisible = false;
                OpenFolderButton.IsVisible = false;
                OpenFileButton.IsVisible = false;
                SettingsButton.IsVisible = false;
                FileList.IsVisible = false;
                ListViewButton.IsVisible = false;
                IsEditorView = true;
                break;
        }
    }
    private void ScrollBelowDocument_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.AllowScrollBelowDocument = !Editor.Options.AllowScrollBelowDocument;
        SaveSettings();
    }
    private void HighlightRowButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.HighlightCurrentLine = !Editor.Options.HighlightCurrentLine;
        SaveSettings();
    }
    private void SpacesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowSpaces = !Editor.Options.ShowSpaces;
        SaveSettings();
    }
    private void TabSpacesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowTabs = !Editor.Options.ShowTabs;
        SaveSettings();
    }
    private void ColumnRulerButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowColumnRulers = !Editor.Options.ShowColumnRulers;
        SaveSettings();
    }
    private void EndOfLineButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowEndOfLine = !Editor.Options.ShowEndOfLine;
        SaveSettings();
    }
    private void ListViewButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (IsEditorView) return;
        FileList.IsVisible = !FileList.IsVisible;
        switch (Editor.GetValue(Grid.ColumnSpanProperty))
        {
            case 1:
                Editor.SetValue(Grid.ColumnSpanProperty, 2);
                break;
            case 2:
                Editor.SetValue(Grid.ColumnSpanProperty, 1);
                break;
        }
        SaveSettings();
    }
    /*private void ListMoveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        switch (Editor.GetValue(Grid.ColumnProperty))
        {
            case 0:
                Editor.SetValue(Grid.ColumnProperty, 1);
                FileList.SetValue(Grid.ColumnProperty, 0);
                //FileList.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Right);
                break;
            case 1:
                Editor.SetValue(Grid.ColumnProperty, 0);
                FileList.SetValue(Grid.ColumnProperty, 1);
                //FileList.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Right);
                break;
        }
    }*/
    private void ViewStatusBarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LocationBar.IsVisible = !LocationBar.IsVisible;
        FileBar.IsVisible = !FileBar.IsVisible;
        SaveSettings();
    }
    private void MoveStatusBarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        switch (LocationBar.GetValue(Grid.RowProperty))
        {
            case 5:
                LocationBar.SetValue(Grid.RowProperty, 3);
                break;
            case 3:
                LocationBar.SetValue(Grid.RowProperty, 5);
                break;
        }
        switch (FileBar.GetValue(Grid.RowProperty))
        {
            case 5:
                FileBar.SetValue(Grid.RowProperty, 3);
                break;
            case 3:
                FileBar.SetValue(Grid.RowProperty, 5);
                break;
        }
        SaveSettings();
    }
}