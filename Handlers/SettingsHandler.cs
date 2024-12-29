using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using Avalonia.Controls;

namespace Enviredit;

public partial class MainWindow : Window
{
    // Define JSON options
    private static readonly JsonSerializerOptions JsonWriteOptions = new()
    {
        WriteIndented = true
    };
    
    // Define functions
    private void GetSettingsFile()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.config\\Enviredit\\");
            SettingsFile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.config\\Enviredit\\Enviredit.json";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/Enviredit/");
            SettingsFile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/Enviredit/Enviredit.json";
        }
    }
    private void FindSettingsFile()
    {
        if (File.Exists(SettingsFile))
        {
            Console.WriteLine("Settings file found");
            Console.WriteLine(SettingsFile);

        }
        else
        {
            Console.WriteLine("No settings file found, creating a new one...");
            using FileStream fileStream = File.Open(SettingsFile, FileMode.Append);
            using StreamWriter file = new StreamWriter(fileStream);
            var themeSetting = new UserSettings
            {
                ThemeSetting = "Default",
                RowHighlightSetting = true
            };
            fileStream.Close();
            var jsonString = JsonSerializer.Serialize(themeSetting);
            using (var writer = new StreamWriter(SettingsFile))
            {
                writer.Write(jsonString);
                writer.Close();
            }
        }
    }
    private static string GetTheme(MainWindow window)
    {
        return (window.RequestedThemeVariant).ToString();
    }
    private void SaveSettings()
    {
        var userSetting = new UserSettings()
        {
            ThemeSetting = GetTheme(this),
            FontFamilySetting = Editor.FontFamily.Name,
            VScrollViewSetting = Editor.VerticalScrollBarVisibility,
            HScrollViewSetting = Editor.HorizontalScrollBarVisibility,
            RectangularEditSetting = Editor.Options.EnableRectangularSelection,
            ScrollBelowDocumentSetting = Editor.Options.AllowScrollBelowDocument,
            RowHighlightSetting = Editor.Options.HighlightCurrentLine,
            GridLinesSetting = MainGrid.ShowGridLines,
            LocationBarSetting = LocationBar.GetValue(Grid.RowProperty),
            LocationBarViewSetting = LocationBar.IsVisible,
            FileBarSetting = FileBar.GetValue(Grid.RowProperty),
            FileBarViewSetting = FileBar.IsVisible,
            SpacesEditorSetting = Editor.Options.ShowSpaces,
            TabSpacesEditorSetting = Editor.Options.ShowTabs,
            ColumnRulerSetting = Editor.Options.ShowColumnRulers,
            EndOfLineSetting = Editor.Options.ShowEndOfLine,
            ListViewSetting = FileList.IsVisible,

            LastUsedFile = FilePath,
            LastUsedFolder = FolderPath
        };
        var jsonString = JsonSerializer.Serialize(userSetting, JsonWriteOptions);
        using (var writer = new StreamWriter(SettingsFile))
        {
            writer.Write(jsonString);
            writer.Close();
        }
        userSetting = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}