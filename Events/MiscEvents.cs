using System;
using Avalonia.Controls;
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
}