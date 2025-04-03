using System;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Enviredit.Views;

public class SettingsClass
{
    public required string OptTheme { get; init; }
    public bool OptHighlightLine { get; init; }
    public bool OptShowSpaces { get; init; }
    public bool OptShowTabs { get; init; }
    public bool OptConvertTabs { get; init; }
    public bool OptShowGridLine { get; init; }
    public int OptIndentSpacing { get; init; }
}
public partial class MainWindow : Window
{
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };
    private void SaveSettings()
    {
        // Set settings
        SettingsClass sc = new SettingsClass
        {
            OptTheme = RequestedThemeVariant.Key.ToString(),
            OptShowSpaces = Editor.Options.ShowSpaces,
            OptHighlightLine = Editor.Options.HighlightCurrentLine,
            OptShowTabs = Editor.Options.ShowTabs,
            OptConvertTabs = Editor.Options.ConvertTabsToSpaces,
            OptShowGridLine = MainGrid.ShowGridLines,
            OptIndentSpacing = Editor.Options.IndentationSize
        };
        // Serialize JSON and write to file
        var settingsInput= JsonSerializer.Serialize(sc, _jsonOptions);
        using var writer = new StreamWriter(LocalGetSettingsFile());
        writer.Write(settingsInput);
    }

    private void LoadSettings()
    {
        // Deserialize JSON
        using var reader = new StreamReader(LocalGetSettingsFile());
        var settingsFileContents = reader.ReadToEnd();
        var settingsOutput = JsonSerializer.Deserialize<SettingsClass>(settingsFileContents);
        if (settingsOutput == null) return;
        // Apply settings
        RequestedThemeVariant = settingsOutput.OptTheme switch
        {
            "Default" => ThemeVariant.Default,
            "Light" => ThemeVariant.Light,
            "Dark" => ThemeVariant.Dark,
            _ => RequestedThemeVariant
        };
        Editor.Options.ShowSpaces = settingsOutput.OptShowSpaces;
        Editor.Options.HighlightCurrentLine = settingsOutput.OptHighlightLine;
        Editor.Options.ShowTabs = settingsOutput.OptShowTabs;
        Editor.Options.ConvertTabsToSpaces = settingsOutput.OptConvertTabs;
        MainGrid.ShowGridLines = settingsOutput.OptShowGridLine;
        Editor.Options.IndentationSize = settingsOutput.OptIndentSpacing;
    }
    private void CreateSettingsFile()
    {
        // If the settings file (~/.config/config.txt) does not exist, create it
        var settingsFileDir= (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/Enviredit/");
        var settingsFile= (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/Enviredit/config.json");
        if (!File.Exists(settingsFile))
        {
            Editor.Options.HighlightCurrentLine = true;
            Editor.Options.ConvertTabsToSpaces = true;
            Console.WriteLine("Settings file not found, Creating...");
            Directory.CreateDirectory(settingsFileDir);
            File.Create(settingsFile);
            LocalSetSettingsFile(settingsFile);
        }
        LocalSetSettingsFile(settingsFile);
    }
}
