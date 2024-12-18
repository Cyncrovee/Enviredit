using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace Enviredit;

public partial class MainWindow : Window
{
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
    // Json serializer write options
    private static readonly JsonSerializerOptions JsonWriteOptions = new()
    {
        WriteIndented = true
    };
    private string _settingsFile = String.Empty;
    public MainWindow()
    {
        InitializeComponent();

        GetSettingsFile();

        var settingsFilePath = String.Empty;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            settingsFilePath = Directory.GetCurrentDirectory() + "\\Enviredit.json";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            settingsFilePath = Directory.GetCurrentDirectory() + "/Enviredit.json";
        }

        // Create settings file if it does not exist
        if (File.Exists(settingsFilePath))
        {
            Console.WriteLine("Settings file found");
            Console.WriteLine(settingsFilePath);

        }
        else
        {
            Console.WriteLine("No settings file found, creating a new one...");
            using FileStream fileStream = File.Open(settingsFilePath, FileMode.Append);
            using StreamWriter file = new StreamWriter(fileStream);
            file.Close();
            var themeSetting = new UserSettings
            {
                ThemeSetting = "Default",
                RowHighlightSetting = true
            };
            var jsonString = JsonSerializer.Serialize(themeSetting);
            var writer = new StreamWriter(_settingsFile);
            writer.Write(jsonString);
            writer.Close();
        }

        // Refresh all settings and checkboxes
        RefreshSettings();
        RefreshIsChecked();

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

    private string _filePath = string.Empty;
    private string _folderPath = string.Empty;
    private bool _isEditorView = false;


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
            RefreshFileInformation();
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
        RefreshList();
    }
    private void LastFolderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var jsonString = File.ReadAllText(_settingsFile);
        var userSettings = JsonSerializer.Deserialize<UserSettings>(jsonString);
        if (userSettings.LastUsedFolder == null) return;
        _folderPath = userSettings.LastUsedFolder;
        FolderPathBlock.Text = "Currently Selected Folder: " + _folderPath;
        RefreshList();
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
            RefreshFileInformation();
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
        SaveSettings();
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
        SaveSettings();
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
        SaveSettings();
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
        SaveSettings();
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
        SaveSettings();
    }
    // "Debug"
    private void GridLinesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        MainGrid.ShowGridLines = !MainGrid.ShowGridLines;
        SaveSettings();
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
                SaveSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            RefreshFileInformation();
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
            RefreshList();
            SaveSettings();
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
        RefreshFileInformation();
        SaveSettings();
    }
    private void LightThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemThemeItem.IsChecked = false;
        LightThemeItem.IsChecked = true;
        DarkThemeItem.IsChecked = false;

        RequestedThemeVariant = ThemeVariant.Light;
        RefreshFileInformation();
        SaveSettings();
    }
    private void DarkThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemThemeItem.IsChecked = false;
        LightThemeItem.IsChecked = false;
        DarkThemeItem.IsChecked = true;

        RequestedThemeVariant = ThemeVariant.Dark;
        RefreshFileInformation();
        SaveSettings();
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
        SaveSettings();
    }


    // Functions for Editor and FileList
    private void EditorCaret_PositionChanged(object? sender, EventArgs e)
    {
        StatusText.Text = "Line: " + Editor.TextArea.Caret.Line + ", Column: " + Editor.TextArea.Caret.Column + " | ";
    }
    private void FileList_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        LoadFromList();
    }


    // User settings functions
    private void GetSettingsFile()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _settingsFile = Directory.GetCurrentDirectory() + "\\Enviredit.json";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            _settingsFile= Directory.GetCurrentDirectory() + "/Enviredit.json";
        }
    }
    private void SaveSettings()
    {
        GetValue(RequestedThemeVariantProperty);
        var userSetting = new UserSettings()
        {
            ThemeSetting = GetValue(RequestedThemeVariantProperty).ToString(),
            FontFamilySetting = Editor.FontFamily.Name,
            RectangularEditSetting = Editor.Options.EnableRectangularSelection,
            ScrollBelowDocumentSetting = Editor.Options.AllowScrollBelowDocument,
            RowHighlightSetting = Editor.Options.HighlightCurrentLine,
            GridLinesSetting = MainGrid.ShowGridLines,
            StatusBarSetting = StatusBar.GetValue(Grid.RowProperty),
            StatusBarViewSetting = StatusBar.IsVisible,
            SpacesEditorSetting = Editor.Options.ShowSpaces,
            TabSpacesEditorSetting = Editor.Options.ShowTabs,
            ColumnRulerSetting = Editor.Options.ShowColumnRulers,
            EndOfLineSetting = Editor.Options.ShowEndOfLine,
            ListViewSetting = FileList.IsVisible,

            LastUsedFile = _filePath,
            LastUsedFolder = _folderPath
        };
        var jsonString = JsonSerializer.Serialize(userSetting, JsonWriteOptions);
        var writer = new StreamWriter(_settingsFile);
        writer.Write(jsonString);
        writer.Close();
    }
    private void RefreshSettings()
    {
        var jsonString = File.ReadAllText(_settingsFile);
        var userSettings = JsonSerializer.Deserialize<UserSettings>(jsonString);
        // Refresh theme setting
        switch (userSettings.ThemeSetting)
        {
            case null:
                RequestedThemeVariant = ThemeVariant.Default;
                break;
            case "Default":
                RequestedThemeVariant = ThemeVariant.Default;
                break;
            case "Light":
                RequestedThemeVariant = ThemeVariant.Light;
                break;
            case "Dark":
                RequestedThemeVariant = ThemeVariant.Dark;
                break;
        }
        // Refresh font family setting
        switch (userSettings.FontFamilySetting)
        {
            case not null:
                foreach (var fontFamily in FontManager.Current.SystemFonts)
                {
                    var fontResult = string.Equals(userSettings.FontFamilySetting, fontFamily.Name);
                    if (fontResult)
                    {
                        Console.WriteLine("Font family found: " + fontFamily.Name);
                        Editor.FontFamily = userSettings.FontFamilySetting;
                        FontFamilyComboBox.PlaceholderText = fontFamily.Name;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Font family not found: " + fontFamily.Name);
                    }
                }
                break;
        }
        // Refresh MenuBar settings
        // Edit
        switch (userSettings.RectangularEditSetting)
        {
            case null:
                Editor.Options.EnableRectangularSelection = true;
                break;
            case true:
                Editor.Options.EnableRectangularSelection = true;
                break;
            case false:
                Editor.Options.EnableRectangularSelection = false;
                break;
        }
        // View
        switch (userSettings.ScrollBelowDocumentSetting)
        {
            case null:
                Editor.Options.AllowScrollBelowDocument = true;
                break;
            case true:
                Editor.Options.AllowScrollBelowDocument = true;
                break;
            case false:
                Editor.Options.AllowScrollBelowDocument = false;
                break;
        }
        switch (userSettings.RowHighlightSetting)
        {
            case null:
                Editor.Options.HighlightCurrentLine = true;
                break;
            case true:
                Editor.Options.HighlightCurrentLine = true;
                break;
            case false:
                Editor.Options.HighlightCurrentLine = false;
                break;
        }
        switch (userSettings.SpacesEditorSetting)
        {
            case null:
                Editor.Options.ShowSpaces = false;
                break;
            case true:
                Editor.Options.ShowSpaces = true;
                break;
            case false:
                Editor.Options.ShowSpaces = false;
                break;
        }
        switch (userSettings.TabSpacesEditorSetting)
        {
            case null:
                Editor.Options.ShowTabs = false;
                break;
            case true:
                Editor.Options.ShowTabs = true;
                break;
            case false:
                Editor.Options.ShowTabs = false;
                break;
        }
        switch (userSettings.ColumnRulerSetting)
        {
            case null:
                Editor.Options.ShowColumnRulers = false;
                break;
            case true:
                Editor.Options.ShowColumnRulers = true;
                break;
            case false:
                Editor.Options.ShowColumnRulers = false;
                break;
        }
        switch (userSettings.EndOfLineSetting)
        {
            case null:
                Editor.Options.ShowEndOfLine = false;
                break;
            case true:
                Editor.Options.ShowEndOfLine = true;
                break;
            case false:
                Editor.Options.ShowEndOfLine = false;
                break;
        }
        switch (userSettings.ListViewSetting)
        {
            case null:
                FileList.IsVisible = true;
                break;
            case true:
                FileList.IsVisible = true;
                break;
            case false:
                FileList.IsVisible = false;
                switch (Editor.GetValue(Grid.ColumnSpanProperty))
                {
                    case 1:
                        Editor.SetValue(Grid.ColumnSpanProperty, 2);
                        break;
                    case 2:
                        Editor.SetValue(Grid.ColumnSpanProperty, 1);
                        break;
                }
                break;
        }
        switch (userSettings.StatusBarViewSetting)
        {
            case null:
                StatusBar.IsVisible = true;
                break;
            case true:
                StatusBar.IsVisible = true;
                break;
            case false:
                StatusBar.IsVisible = false;
                break;
        }
        switch (userSettings.StatusBarSetting)
        {
            case null:
                StatusBar.SetValue(Grid.RowProperty, 5);
                break;
            case 5:
                StatusBar.SetValue(Grid.RowProperty, 5);
                break;
            case 3:
                StatusBar.SetValue(Grid.RowProperty, 3);
                break;
        }
        // Debug
        switch (userSettings.GridLinesSetting)
        {
            case null:
                MainGrid.ShowGridLines = false;
                break;
            case true:
                MainGrid.ShowGridLines = true;
                break;
            case false:
                MainGrid.ShowGridLines = false;
                break;
        }
    }
    private void RefreshIsChecked()
    {
        // Refresh theme setting checkbox
        switch (GetValue(RequestedThemeVariantProperty).ToString())
        {
            case "Default":
                SystemThemeItem.IsChecked = true;
                DarkThemeItem.IsChecked = false;
                LightThemeItem.IsChecked = false;
                break;
            case "Dark":
                SystemThemeItem.IsChecked = false;
                DarkThemeItem.IsChecked = true;
                LightThemeItem.IsChecked = false;
                break;
            case "Light":
                SystemThemeItem.IsChecked = false;
                DarkThemeItem.IsChecked = false;
                LightThemeItem.IsChecked = true;
                break;
        }
        // Refresh Edit settings checkboxes
        switch (Editor.Options.EnableRectangularSelection)
        {
            case true:
                ToggleRectangularSelectionButton.IsChecked = true;
                break;
            case false:
                ToggleRectangularSelectionButton.IsChecked = false;
                break;
        }
        // Refresh View settings checkboxes
        switch (Editor.Options.AllowScrollBelowDocument)
        {
            case true:
                ScrollBelowDocumentButton.IsChecked = true;
                break;
            case false:
                ScrollBelowDocumentButton.IsChecked = false;
                break;
        }
        switch (Editor.Options.HighlightCurrentLine)
        {
            case true:
                HighlightRowButton.IsChecked = true;
                break;
            case false:
                HighlightRowButton.IsChecked = false;
                break;
        }
        switch (Editor.Options.ShowSpaces)
        {
            case true:
                SpacesButton.IsChecked = true;
                break;
            case false:
                SpacesButton.IsChecked = false;
                break;
        }
        switch (Editor.Options.ShowTabs)
        {
            case true:
                TabSpacesButton.IsChecked = true;
                break;
            case false:
                TabSpacesButton.IsChecked = false;
                break;
        }
        switch (Editor.Options.ShowColumnRulers)
        {
            case true:
                ColumnRulerButton.IsChecked = true;
                break;
            case false:
                ColumnRulerButton.IsChecked = false;
                break;
        }
        switch (Editor.Options.ShowEndOfLine)
        {
            case true:
                EndOfLineButton.IsChecked = true;
                break;
            case false:
                EndOfLineButton.IsChecked = false;
                break;
        }
        switch (FileList.IsVisible)
        {
            case true:
                ListViewButton.IsChecked = true;
                break;
            case false:
                ListViewButton.IsChecked = false;
                break;
        }
        switch (StatusBar.IsVisible)
        {
            case true:
                ViewStatusBarButton.IsChecked = true;
                break;
            case false:
                ViewStatusBarButton.IsChecked = false;
                break;
        }
        // Refresh Debug settings checkboxes
        switch (MainGrid.ShowGridLines)
        {
            case true:
                GridLinesButton.IsChecked = true;
                break;
            case false:
                GridLinesButton.IsChecked = false;
                break;
        }
    }


    // Misc functions
    private void LoadFromList()
    {
        if (FileList.SelectedItem != null)
        {
            var selectedFile = FileList.SelectedItem.ToString();
            _filePath = selectedFile;
            Editor.Clear();

            try
            {
                using StreamReader reader = new(selectedFile);
                var text = reader.ReadToEnd();
                Editor.Text = text;
                reader.Close();
                FilePathBlock.Text = "Currently Selected File: " + selectedFile;
                SaveSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        else
        {
            Console.WriteLine("No file selected");
        }
        RefreshFileInformation();
    }
    private void RefreshList()
    {
        if (_folderPath != string.Empty)
        {
            string[] files = Directory.GetFiles(_folderPath);

            FileList.Items.Clear();
            foreach (string file in files)
            {
                FileList.Items.Add(file);
            }
            FolderPathBlock.Text = "Currently Selected File: " + _folderPath;
        }
        else
        {
            Console.WriteLine("No folder selected");
        }
    }
    private void RefreshFileInformation()
    {
        try
        {
            if (ActualThemeVariant == ThemeVariant.Default)
            {
                var textEditor = this.FindControl<TextEditor>("Editor");
                var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
                var textMateInstallation = textEditor.InstallTextMate(registryOptions);
                string languageExtension = registryOptions.GetLanguageByExtension(Path.GetExtension(_filePath)).Id;

                textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(languageExtension));
                LanguageStatusText.Text= ("Language: " + languageExtension.ToUpper());
            }
            else if (ActualThemeVariant == ThemeVariant.Light)
            {
                var textEditor = this.FindControl<TextEditor>("Editor");
                var registryOptions = new RegistryOptions(ThemeName.LightPlus);
                var textMateInstallation = textEditor.InstallTextMate(registryOptions);
                string languageExtension = registryOptions.GetLanguageByExtension(Path.GetExtension(_filePath)).Id;

                textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(languageExtension));
                LanguageStatusText.Text= ("Language: " + languageExtension.ToUpper());
            }
            else if (ActualThemeVariant == ThemeVariant.Dark)
            {
                var textEditor = this.FindControl<TextEditor>("Editor");
                var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
                var textMateInstallation = textEditor.InstallTextMate(registryOptions);
                string languageExtension = registryOptions.GetLanguageByExtension(Path.GetExtension(_filePath)).Id;

                textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(languageExtension));
                LanguageStatusText.Text= ("Language: " + languageExtension.ToUpper());
            }
        }
        catch (Exception)
        {
            LanguageStatusText.Text= ("Language: " + "Not a Programming Language/ Language Not Supported/ No file selected");
            Console.WriteLine("Not a Programming Language/Language Not Supported/ No file selected");
        }
        var fileExtension = Path.GetExtension(_filePath);
        FileExtensionText.Text = ("File Extension: " + fileExtension + " | ");
    }
}
