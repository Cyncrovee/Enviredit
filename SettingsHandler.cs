using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using Avalonia.Controls;

namespace Enviredit;

public class SettingsHandler
{
    // Define classes
    private static readonly JsonSerializerOptions JsonWriteOptions = new()
    {
        WriteIndented = true
    };
    private class UserSettings
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
    
    
    // Define functions
    public void SettingsFile(MainWindow window)
    {
        if (File.Exists(window._settingsFile))
        {
            Console.WriteLine("Settings file found");
            Console.WriteLine(window._settingsFile);

        }
        else
        {
            Console.WriteLine("No settings file found, creating a new one...");
            //using FileStream fileCreate = File.Create(window._settingsFile);
            using FileStream fileStream = File.Open(window._settingsFile, FileMode.Append);
            using StreamWriter file = new StreamWriter(fileStream);
            file.Close();
            var themeSetting = new MainWindow.UserSettings
            {
                ThemeSetting = "Default",
                RowHighlightSetting = true
            };
            var jsonString = JsonSerializer.Serialize(themeSetting);
            var writer = new StreamWriter(window._settingsFile);
            writer.Write(jsonString);
            writer.Close();
            //fileCreate.Close();
        }
    }
    public void GetSettingsFile(MainWindow window)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Enviredit\\Enviredit.json");
            window._settingsFile = Environment.SpecialFolder.UserProfile + "\\Enviredit.json";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/Enviredit/");
            window._settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/Enviredit/Enviredit.json";
        }
    }
    private static string GetTheme(TopLevel topLevel)
    {
        return (topLevel.RequestedThemeVariant).ToString();
    }
    public void SaveSettings(MainWindow window)
    {
        var userSetting = new UserSettings()
        {
            ThemeSetting = GetTheme(window),
            FontFamilySetting = window.Editor.FontFamily.Name,
            RectangularEditSetting = window.Editor.Options.EnableRectangularSelection,
            ScrollBelowDocumentSetting = window.Editor.Options.AllowScrollBelowDocument,
            RowHighlightSetting = window.Editor.Options.HighlightCurrentLine,
            GridLinesSetting = window.MainGrid.ShowGridLines,
            StatusBarSetting = window.StatusBar.GetValue(Grid.RowProperty),
            StatusBarViewSetting = window.StatusBar.IsVisible,
            SpacesEditorSetting = window.Editor.Options.ShowSpaces,
            TabSpacesEditorSetting = window.Editor.Options.ShowTabs,
            ColumnRulerSetting = window.Editor.Options.ShowColumnRulers,
            EndOfLineSetting = window.Editor.Options.ShowEndOfLine,
            ListViewSetting = window.FileList.IsVisible,

            LastUsedFile = window._filePath,
            LastUsedFolder = window._folderPath
        };
        var jsonString = JsonSerializer.Serialize(userSetting, JsonWriteOptions);
        var writer = new StreamWriter(window._settingsFile);
        writer.Write(jsonString);
        writer.Close();
    }
}