using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Enviredit;

public partial class MainWindow : Window
{
    private void EditorCaret_PositionChanged(object? sender, EventArgs e)
    {
        double x = Editor.TextArea.Caret.Line;
        double y = Editor.TextArea.Document.LineCount;
        StatusText.Text =  "Line: " + Editor.TextArea.Caret.Line + ", Column: " + Editor.TextArea.Caret.Column + " | " + Convert.ToInt32(x / y * 100) + "%";
    }
    private void FileList_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (FileList.SelectedItem == null) return;
        FilePath = FileList.SelectedItem.ToString();
        LoadFile();
        RefreshFileInformation();
        SaveSettings();
    }
    private void VScrollBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        switch (VScrollBox.SelectedIndex)
        {
            case 0:
                Editor.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                break;
            case 1:
                Editor.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                break;
            case 2:
                Editor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                break;
            case 3:
                Editor.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                break;
        }
        SaveSettings();
    }
    private void HScrollBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        switch (HScrollBox.SelectedIndex)
        {
            case 0:
                Editor.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                break;
            case 1:
                Editor.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                break;
            case 2:
                Editor.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                break;
            case 3:
                Editor.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                break;
        }
        SaveSettings();
    }
}