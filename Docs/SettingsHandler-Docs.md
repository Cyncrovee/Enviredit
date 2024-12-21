# Docs for SettingsHandler.cs

SettingsHandler communicates with MainWindow to handle various user settings, for example whether the user has row highlighting enabled.

## Initial Options
### JsonSerialzerOptions
Near the top of the file, the SettingsHandler initialises the JSON write options. in this case it simply sets WriteIndented to true.
```csharp
private static readonly JsonSerializerOptions JsonWriteOptions = new()
{
    WriteIndented = true
};
```

### UserSettings
Next, it initialises the UserSettings class, which will hold the user settings so they can later be written to the JSON settings file.
```csharp
private class UserSettings
{
    // Define theme setting
    public string? ThemeSetting { get; set; }
    //Define font family setting
    public string? FontFamilySetting { get; set; }
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
    public bool? LocationBarViewSetting { get; set; }
    public bool? FileBarViewSetting { get; set; }
    public bool? FileViewSetting { get; set; }
    public int? LocationBarSetting { get; set; }
    public int? FileBarSetting { get; set; }
    // Define Debug settings
    public bool? GridLinesSetting { get; set; }
}
```

## Functions
### GetSettingsFile Function
Now we get to the functions, the first of which being the GetSettingsFile Function. This function will detect the users operating system, then create the directory the settings file will be stored and set the _settingsFile variable to the appropriate path.
```csharp
public void GetSettingsFile(MainWindow window)
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.config\\Enviredit\\");
        window._settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.config\\Enviredit\\Enviredit.json";
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/Enviredit/");
        window._settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/Enviredit/Enviredit.json";
    }
}
```

### SettingsFile Function
However, we still don't know if the settings file even exits yet. That what the SettingsFile function is for. If the file exists, it will simply print that the file has been found, and print the path to it. Otherwise, it will print that the file was not found, and attempt to create one with some basic settings already set.
```csharp
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
    }
}
```

### GetTheme and SaveSettings Functions
Here, the SaveSettings function saves the user settings to a JSON file, based on the UserSettings class and uses the previously set JsonWriteOptions. The GetTheme function exists the get the current theme from the top level, so SaveSettings can write it to the file.
```csharp
private static string GetTheme(TopLevel topLevel)
{
    return (topLevel.RequestedThemeVariant).ToString();
}
public void SaveSettings(MainWindow window)
{
    Console.WriteLine(window.Editor.FontSize);
    var userSetting = new UserSettings()
    {
        ThemeSetting = GetTheme(window),
        FontFamilySetting = window.Editor.FontFamily.Name,
        RectangularEditSetting = window.Editor.Options.EnableRectangularSelection,
        ScrollBelowDocumentSetting = window.Editor.Options.AllowScrollBelowDocument,
        RowHighlightSetting = window.Editor.Options.HighlightCurrentLine,
        GridLinesSetting = window.MainGrid.ShowGridLines,
        LocationBarSetting = window.LocationBar.GetValue(Grid.RowProperty),
        LocationBarViewSetting = window.LocationBar.IsVisible,
        FileBarSetting = window.FileBar.GetValue(Grid.RowProperty),
        FileBarViewSetting = window.FileBar.IsVisible,
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
```