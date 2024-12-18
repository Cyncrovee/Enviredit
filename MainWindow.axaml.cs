using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;

namespace Enviredit;

public partial class MainWindow : Window
{
    public SettingsHandler settingsHandler = new SettingsHandler();
    RefreshHandler refreshHandler = new RefreshHandler();
    public class UserSettings
    {
        // Define theme setting
        public string? ThemeSetting { get; set; }
        //Define font family setting
        public string? FontFamilySetting { get; set; }
        // Define MenuBar settings
        // Define File settings
        public string? LastUsedFile { get; set; }
        public string? LastUsedFolder { get; set; }
        // Define Edit settings
        public bool? RectangularEditSetting { get; set; }
        // Define View settings
        public bool? ScrollBelowDocumentSetting { get; set; }
        public bool? RowHighlightSetting { get; set; }
        public bool? SpacesEditorSetting { get; set; }
        public bool? TabSpacesEditorSetting { get; set; }
        public bool? ColumnRulerSetting { get; set; }
        public bool? EndOfLineSetting { get; set; }
        public bool? ListViewSetting { get; set; }
        public bool? StatusBarViewSetting { get; set; }
        public int? StatusBarSetting { get; set; }
        // Define Debug settings
        public bool? GridLinesSetting { get; set; }
    }
    public string _settingsFile = string.Empty;
    public string _filePath = string.Empty;
    public string _folderPath = string.Empty;
    public bool _isEditorView;
    public MainWindow()
    {
        InitializeComponent();

        settingsHandler.GetSettingsFile(this);
        settingsHandler.SettingsFile(this);

        // Refresh all settings and checkboxes
        refreshHandler.RefreshSettings(this);
        refreshHandler.RefreshIsChecked(this);

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
        if (_filePath != string.Empty)
        {
            var writer = new StreamWriter(_filePath);
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
            _filePath = file.Path.LocalPath;
            FilePathBlock.Text = "Currently Selected File: " + _filePath;
            refreshHandler.RefreshFileInformation(this);
        }
        else
        {
            Console.WriteLine("No file created");
        }
    }
    private void OpenContainingFolder_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_filePath == string.Empty) return;
        FileInfo fileInfo = new FileInfo(_filePath);
        var directoryPath = fileInfo.Directory;
        _folderPath = directoryPath.FullName;
        refreshHandler.RefreshList(this);
    }
    private void LastFolderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var jsonString = File.ReadAllText(_settingsFile);
        var userSettings = JsonSerializer.Deserialize<UserSettings>(jsonString);
        if (userSettings.LastUsedFolder == null) return;
        _folderPath = userSettings.LastUsedFolder;
        FolderPathBlock.Text = "Currently Selected Folder: " + _folderPath;
        refreshHandler.RefreshList(this);
    }
    private void LastFileButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var jsonString = File.ReadAllText(_settingsFile);
        var userSettings = JsonSerializer.Deserialize<UserSettings>(jsonString);
        if (userSettings.LastUsedFile == null) return;
        _filePath = userSettings.LastUsedFile;
        try
        {
            using StreamReader reader = new(_filePath);
            var text = reader.ReadToEnd();
            Editor.Text = text;
            reader.Close();
            FilePathBlock.Text = "Currently Selected File: " + _filePath;
            refreshHandler.RefreshFileInformation(this);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private void ExitFileFolder_OnClick(object? sender, RoutedEventArgs e)
    {
        _filePath = string.Empty;
        _folderPath = string.Empty;

        FileList.Items.Clear();

        FolderPathBlock.Text = "Currently Selected Folder: ";
        FilePathBlock.Text = "Currently Selected File: ";
    }
    private void Exit(object? sender, RoutedEventArgs e)
    {
        settingsHandler.SaveSettings(this);;
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
        if (Clipboard == null || _folderPath == string.Empty) return;
        Clipboard.SetTextAsync(_folderPath);
    }
    private void CopyFilePathButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Clipboard == null || _filePath == string.Empty) return;
        Clipboard.SetTextAsync(_filePath);
    }
    private void Clear(object? sender, RoutedEventArgs e)
    {
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
        settingsHandler.SaveSettings(this);
        //settingsHandler.SaveSettings(this);;
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
        switch (_isEditorView)
        {
            case true:
                FolderPathBlock.IsVisible = true;
                FilePathBlock.IsVisible = true;
                OpenFolderButton.IsVisible = true;
                OpenFileButton.IsVisible = true;
                SettingsButton.IsVisible = true;
                FileList.IsVisible = true;
                ListViewButton.IsChecked = true;
                ListViewButton.IsVisible = true;
                _isEditorView = false;
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
                ListViewButton.IsChecked = false;
                ListViewButton.IsVisible = false;
                _isEditorView = true;
                break;
        }
    }
    private void ScrollBelowDocument_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.AllowScrollBelowDocument = !Editor.Options.AllowScrollBelowDocument;
        settingsHandler.SaveSettings(this);;
    }
    private void HighlightRowButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.HighlightCurrentLine = !Editor.Options.HighlightCurrentLine;
        settingsHandler.SaveSettings(this);;
    }
    private void SpacesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowSpaces = !Editor.Options.ShowSpaces;
        settingsHandler.SaveSettings(this);;
    }
    private void TabSpacesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowTabs = !Editor.Options.ShowTabs;
        settingsHandler.SaveSettings(this);;
    }
    private void ColumnRulerButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowColumnRulers = !Editor.Options.ShowColumnRulers;
        settingsHandler.SaveSettings(this);;
    }
    private void EndOfLineButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Editor.Options.ShowEndOfLine = !Editor.Options.ShowEndOfLine;
        settingsHandler.SaveSettings(this);;
    }
    private void ListViewButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_isEditorView) return;
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
        settingsHandler.SaveSettings(this);;
    }
    private void ListMoveButton_OnClick(object? sender, RoutedEventArgs e)
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
    }
    private void ViewStatusBarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        StatusBar.IsVisible = !StatusBar.IsVisible;
        settingsHandler.SaveSettings(this);;
    }
    private void MoveStatusBarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        switch (StatusBar.GetValue(Grid.RowProperty))
        {
            case 5:
                StatusBar.SetValue(Grid.RowProperty, 3);
                break;
            case 3:
                StatusBar.SetValue(Grid.RowProperty, 5);
                break;
        }
        settingsHandler.SaveSettings(this);;
    }
    // "Debug"
    private void GridLinesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        MainGrid.ShowGridLines = !MainGrid.ShowGridLines;
        settingsHandler.SaveSettings(this);;
    }


    // Functions for right side buttons
    private async void OpenFileButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var file = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open File",
            AllowMultiple = false
        });

        if (file.Count >= 1)
        {
            _filePath = file.First().Path.LocalPath;
            FilePathBlock.Text = "Currently Selected File: " + _filePath;

            string selectedFile = _filePath;
            Editor.Clear();
            try
            {
                using StreamReader reader = new(selectedFile);
                var text = reader.ReadToEndAsync().Result;
                Editor.Text = text;
                settingsHandler.SaveSettings(this);;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            refreshHandler.RefreshFileInformation(this);
        }
        else
        {
            Console.WriteLine("No file selected");
        }
    }
    private async void OpenFolderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Open Folder",
            AllowMultiple = false
        });

        if (folder.Count >= 1)
        {
            _folderPath = folder[0].Path.LocalPath;
            FolderPathBlock.Text = "Currently Selected File: " + _folderPath;
            refreshHandler.RefreshList(this);
            settingsHandler.SaveSettings(this);;
        }
        else
        {
            Console.WriteLine("No folder selected");
        }
    }
    private void SystemThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemThemeItem.IsChecked = true;
        LightThemeItem.IsChecked = false;
        DarkThemeItem.IsChecked = false;

        RequestedThemeVariant = ThemeVariant.Default;
        refreshHandler.RefreshFileInformation(this);
        settingsHandler.SaveSettings(this);;
    }
    private void LightThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemThemeItem.IsChecked = false;
        LightThemeItem.IsChecked = true;
        DarkThemeItem.IsChecked = false;

        RequestedThemeVariant = ThemeVariant.Light;
        refreshHandler.RefreshFileInformation(this);
        settingsHandler.SaveSettings(this);;
    }
    private void DarkThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemThemeItem.IsChecked = false;
        LightThemeItem.IsChecked = false;
        DarkThemeItem.IsChecked = true;

        RequestedThemeVariant = ThemeVariant.Dark;
        refreshHandler.RefreshFileInformation(this);
        settingsHandler.SaveSettings(this);;
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
        settingsHandler.SaveSettings(this);;
    }


    // Functions for Editor and FileList
    private void EditorCaret_PositionChanged(object? sender, EventArgs e)
    {
        StatusText.Text = "Line: " + Editor.TextArea.Caret.Line + ", Column: " + Editor.TextArea.Caret.Column + " | ";
    }
    private void FileList_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        refreshHandler.LoadFromList(this);
    }
}
