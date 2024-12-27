using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Media;

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
    //public readonly SettingsHandler MainSettingsHandler = new();
    //private readonly RefreshHandler _mainRefreshHandler = new();
    //private readonly FileHandler _fileHandler = new();
    //private readonly FolderHandler _folderHandler = new();
    public MainWindow()
    {
        InitializeComponent();

        GetSettingsFile();
        FindSettingsFile();

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
    private void FontFamilyComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (FontFamilyComboBox.SelectedItem == null) return;
        Editor.FontFamily = FontFamily.Parse(FontFamilyComboBox.SelectedItem.ToString());
        SaveSettings();
    }
}
