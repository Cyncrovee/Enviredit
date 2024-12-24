using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Enviredit.Handlers;

namespace Enviredit;

public partial class MainWindow : INotifyPropertyChanged
{
    // Define variables for data binding
    private string _folderPath = String.Empty;
    private string _filePath = String.Empty;
    private string _encoding = String.Empty;
    private string _extension = String.Empty;
    private string _language = String.Empty;

    public string FolderPath
    {
        get
        {
            return _folderPath;
        }
        set
        {
            if (_folderPath != value)
            {
                _folderPath = value;
                NotifyPropertyChanged();
            }
        }
    }
    public string FilePath
    {
        get
        {
            return _filePath;
        }
        set
        {
            if (_filePath != value)
            {
                _filePath = value;
                NotifyPropertyChanged();
            }
        }
    }
    public string Encoding
    {
        get
        {
            return _encoding;
        }
        set
        {
            if (_encoding != value)
            {
                _encoding = value;
                NotifyPropertyChanged();
            }
        }
    }
    public string Extension
    {
        get
        {
            return _extension;
        }
        set
        {
            if (_extension != value)
            {
                _extension = value;
                NotifyPropertyChanged();
            }
        }
    }
    public string Language
    {
        get
        {
            return _language;
        }
        set
        {
            if (_language != value)
            {
                _language = value;
                NotifyPropertyChanged();
            }
        }
    }
    public new event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public partial class MainWindow : Window
{
    public class UserSettings
    {
        // Define theme setting
        public string? ThemeSetting { get; init; }
        //Define font family setting
        public string? FontFamilySetting { get; init; }
        // Define MenuBar settings
        // Define File settings
        public string? LastUsedFile { get; init; }
        public string? LastUsedFolder { get; init; }
        // Define Edit settings
        public bool? RectangularEditSetting { get; init; }
        // Define View settings
        public bool? ScrollBelowDocumentSetting { get; init; }
        public bool? RowHighlightSetting { get; init; }
        public bool? SpacesEditorSetting { get; init; }
        public bool? TabSpacesEditorSetting { get; init; }
        public bool? ColumnRulerSetting { get; init; }
        public bool? EndOfLineSetting { get; init; }
        public bool? ListViewSetting { get; init; }
        public bool? LocationBarViewSetting { get; init; }
        public bool? FileBarViewSetting { get; init; }
        public bool? FileViewSetting { get; init; }
        public int? LocationBarSetting { get; init; }
        public int? FileBarSetting { get; init; }
        // Define Debug settings
        public bool? GridLinesSetting { get; init; }
    }
    public string SettingsFile { get; set; }
    private bool IsEditorView { get; set; }
    public readonly SettingsHandler MainSettingsHandler = new();
    private readonly RefreshHandler _mainRefreshHandler = new();
    private readonly FileHandler _fileHandler = new();
    private readonly FolderHandler _folderHandler = new();
    public MainWindow()
    {
        InitializeComponent();

        MainSettingsHandler.GetSettingsFile(this);
        MainSettingsHandler.SettingsFile(this);

        // Refresh all settings and checkboxes
        _mainRefreshHandler.RefreshSettings(this);
        _mainRefreshHandler.RefreshIsChecked(this);

        // Set options for Editor
        Editor.Options.EnableRectangularSelection = true;
        Editor.Options.AllowToggleOverstrikeMode = true;
        Editor.TextArea.Caret.PositionChanged += EditorCaret_PositionChanged;

        // Add indentation sizes to IndentationSizeComboBox
        int[] indentationSizes = Enumerable.Range(1, 64).ToArray();
        foreach (var  indentationSize in indentationSizes)
        {
            IndentationSizeComboBox.Items.Add(indentationSize);
        }
        // Add font families to FontFamilyComboBox
        foreach (var font in FontManager.Current.SystemFonts.OrderBy(f => f.Name))
        {
            FontFamilyComboBox.Items.Add(font);
        }
    }

    // MenuBar functions
    // "File"
    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (FilePath != string.Empty)
        {
            var writer = new StreamWriter(FilePath);
            await writer.WriteAsync(Editor.Text);
            writer.Close();
            Console.WriteLine("File saved");
        }
        else
        {
            Console.WriteLine("No file selected");
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
            await using var stream = await file.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(Editor.Text);
            FilePath = file.Path.LocalPath;
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
    // "Edit"
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
        if (Clipboard == null || FolderPath == string.Empty) return;
        Clipboard.SetTextAsync(FolderPath);
    }
    private void CopyFilePathButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Clipboard == null || FilePath == string.Empty) return;
        Clipboard.SetTextAsync(FilePath);
    }
    private void Clear(object? sender, RoutedEventArgs e)
    {
        Editor.Text= string.Empty;
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
        MainSettingsHandler.SaveSettings(this);
    }
    // "View"
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
        MainSettingsHandler.SaveSettings(this);
    }
    private void HighlightRowButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.HighlightCurrentLine = !Editor.Options.HighlightCurrentLine;
        MainSettingsHandler.SaveSettings(this);;
    }
    private void SpacesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowSpaces = !Editor.Options.ShowSpaces;
        MainSettingsHandler.SaveSettings(this);;
    }
    private void TabSpacesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowTabs = !Editor.Options.ShowTabs;
        MainSettingsHandler.SaveSettings(this);;
    }
    private void ColumnRulerButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowColumnRulers = !Editor.Options.ShowColumnRulers;
        MainSettingsHandler.SaveSettings(this);;
    }
    private void EndOfLineButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowEndOfLine = !Editor.Options.ShowEndOfLine;
        MainSettingsHandler.SaveSettings(this);;
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
        MainSettingsHandler.SaveSettings(this);;
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
        MainSettingsHandler.SaveSettings(this);;
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
        MainSettingsHandler.SaveSettings(this);;
    }
    // "Debug"
    private void GridLinesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        MainGrid.ShowGridLines = !MainGrid.ShowGridLines;
        MainSettingsHandler.SaveSettings(this);;
    }


    // Functions for right side buttons
    private async void OpenFileButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await _fileHandler.OpenFileDialog(this);
        _fileHandler.LoadFile(this);
        _mainRefreshHandler.RefreshFileInformation(this);
    }
    private async void OpenFolderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await _folderHandler.OpenFolderDialog(this);
        _mainRefreshHandler.RefreshList(this);
        MainSettingsHandler.SaveSettings(this);
    }
    private void SystemThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemThemeItem.IsChecked = true;
        LightThemeItem.IsChecked = false;
        DarkThemeItem.IsChecked = false;

        RequestedThemeVariant = ThemeVariant.Default;
        _mainRefreshHandler.RefreshFileInformation(this);
        MainSettingsHandler.SaveSettings(this);;
    }
    private void LightThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemThemeItem.IsChecked = false;
        LightThemeItem.IsChecked = true;
        DarkThemeItem.IsChecked = false;

        RequestedThemeVariant = ThemeVariant.Light;
        _mainRefreshHandler.RefreshFileInformation(this);
        MainSettingsHandler.SaveSettings(this);;
    }
    private void DarkThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemThemeItem.IsChecked = false;
        LightThemeItem.IsChecked = false;
        DarkThemeItem.IsChecked = true;

        RequestedThemeVariant = ThemeVariant.Dark;
        _mainRefreshHandler.RefreshFileInformation(this);
        MainSettingsHandler.SaveSettings(this);;
    }
    private void IndentationSizeComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IndentationSizeComboBox.SelectedItem == null) return;
        Editor.Options.IndentationSize = IndentationSizeComboBox.SelectedIndex + 1;
        Console.WriteLine(Editor.Options.IndentationSize);
    }
    private void FontFamilyComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (FontFamilyComboBox.SelectedItem == null) return;
        Editor.FontFamily = FontFamily.Parse(FontFamilyComboBox.SelectedItem.ToString());
        MainSettingsHandler.SaveSettings(this);;
    }


    // Functions for Editor and FileList
    private void EditorCaret_PositionChanged(object? sender, EventArgs e)
    {
        double x = Editor.TextArea.Caret.Line;
        double y = Editor.TextArea.Document.LineCount;
        StatusText.Text =  "Line: " + Editor.TextArea.Caret.Line + ", Column: " + Editor.TextArea.Caret.Column + " | " + Convert.ToInt32(x / y * 100) + "%";
    }
    private void FileList_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (FileList.SelectedItem != null)
        {
            FilePath = FileList.SelectedItem.ToString();
            _fileHandler.LoadFile(this);
            _mainRefreshHandler.RefreshFileInformation(this);
            MainSettingsHandler.SaveSettings(this);
        }
    }

    private async void About_OnClick(object? sender, RoutedEventArgs e)
    {
        var ownerWindow = this;
        var dialogWindow = new AboutWindow();
        dialogWindow.RequestedThemeVariant = RequestedThemeVariant;
        await dialogWindow.ShowDialog(ownerWindow);
    }
}
