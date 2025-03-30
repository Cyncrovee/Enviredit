using System;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using Enviredit.ViewModels;
using Tmds.DBus.Protocol;

namespace Enviredit.Views;

public class SettingsClass
{
    public bool OptHighlightLine { get; set; }
    public bool OptShowSpaces { get; set; }
    public bool OptShowTabs { get; set; }
    public bool OptConvertTabs { get; set; }
    public bool OptShowGridLine { get; set; }
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
            OptShowSpaces = Editor.Options.ShowSpaces,
            OptHighlightLine = Editor.Options.HighlightCurrentLine,
            OptShowTabs = Editor.Options.ShowTabs,
            OptConvertTabs = Editor.Options.ConvertTabsToSpaces,
            OptShowGridLine = MainGrid.ShowGridLines
        };
        // Serialize JSON and write to file
        var settingsInput= JsonSerializer.Serialize(sc, _jsonOptions);
        using StreamWriter writer = new StreamWriter(LocalGetSettingsFile());
        writer.Write(settingsInput);
    }

    private void LoadSettings()
    {
        using StreamReader reader = new StreamReader(LocalGetSettingsFile());
        var settingsFileContents = reader.ReadToEnd();
        var settingsOutput = JsonSerializer.Deserialize<SettingsClass>(settingsFileContents);
        if (settingsOutput == null) return;
        Editor.Options.ShowSpaces = settingsOutput.OptShowSpaces;
        Editor.Options.HighlightCurrentLine = settingsOutput.OptHighlightLine;
        Editor.Options.ShowTabs = settingsOutput.OptShowTabs;
        Editor.Options.ConvertTabsToSpaces = settingsOutput.OptConvertTabs;
        MainGrid.ShowGridLines = settingsOutput.OptShowGridLine;
    }
    private void CreateSettingsFile()
    {
        // If the settings file (config.txt) does not exist, create it
        var settingsFileDir= (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/Enviredit/");
        var settingsFile= (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/Enviredit/config.json");
        if (!File.Exists(settingsFile))
        {
            Editor.Options.HighlightCurrentLine = true;
            Editor.Options.ConvertTabsToSpaces = true;
            Console.WriteLine("Settings file not found, Creating...");
            Directory.CreateDirectory(settingsFileDir);
            File.Create(settingsFile);
        }
        else
        {
            Console.WriteLine("Settings file found");
        }
        LocalSetSettingsFile(settingsFile);
    }
}
