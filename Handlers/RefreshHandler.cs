using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace Enviredit;

public class RefreshHandler
{
    public void RefreshSettings(MainWindow window)
    {
        var jsonString = File.ReadAllText(window.SettingsFile);
        var userSettings = JsonSerializer.Deserialize<MainWindow.UserSettings>(jsonString);
        // Refresh theme setting
        switch (userSettings.ThemeSetting)
        {
            case null:
                window.RequestedThemeVariant = ThemeVariant.Default;
                break;
            case "Default":
                window.RequestedThemeVariant = ThemeVariant.Default;
                break;
            case "Light":
                window.RequestedThemeVariant = ThemeVariant.Light;
                break;
            case "Dark":
                window.RequestedThemeVariant = ThemeVariant.Dark;
                break;
        }
        // Refresh font family setting
        switch (userSettings.FontFamilySetting)
        {
            case not null:
                List<FontFamily> notFoundFonts = new List<FontFamily>();
                foreach (var fontFamily in FontManager.Current.SystemFonts)
                {
                    var fontResult = string.Equals(userSettings.FontFamilySetting, fontFamily.Name);
                    if (fontResult)
                    {
                        Console.WriteLine("Font family found: " + fontFamily.Name);
                        window.Editor.FontFamily = userSettings.FontFamilySetting;
                        window.FontFamilyComboBox.PlaceholderText = fontFamily.Name;
                        break;
                    }
                    else
                    {
                        notFoundFonts.Add(fontFamily);
                    }
                }
                Console.WriteLine("Not found " + notFoundFonts.Count + " fonts");
                break;
        }
        // Refresh MenuBar settings
        // Edit
        switch (userSettings.RectangularEditSetting)
        {
            default:
                window.Editor.Options.EnableRectangularSelection = userSettings.RectangularEditSetting.Value;
                break;
            case null:
                window.Editor.Options.EnableRectangularSelection = true;
                break;
        }
        // View
        switch (userSettings.ScrollBelowDocumentSetting)
        {
            default:
                window.Editor.Options.AllowScrollBelowDocument = userSettings.ScrollBelowDocumentSetting.Value;
                break;
            case null:
                window.Editor.Options.AllowScrollBelowDocument = true;
                break;
                
        }
        switch (userSettings.RowHighlightSetting)
        {
            default:
                window.Editor.Options.HighlightCurrentLine = userSettings.RowHighlightSetting.Value;
                break;
            case null:
                window.Editor.Options.HighlightCurrentLine = true;
                break;
        }
        switch (userSettings.SpacesEditorSetting)
        {
            default:
                window.Editor.Options.ShowSpaces = userSettings.SpacesEditorSetting.Value;
                break;
            case null:
                window.Editor.Options.ShowSpaces = false;
                break;
        }
        switch (userSettings.TabSpacesEditorSetting)
        {
            default:
                window.Editor.Options.ShowTabs = userSettings.TabSpacesEditorSetting.Value;
                break;
            case null:
                window.Editor.Options.ShowTabs = false;
                break;
        }
        switch (userSettings.ColumnRulerSetting)
        {
            default:
                window.Editor.Options.ShowColumnRulers = userSettings.ColumnRulerSetting.Value;
                break;
            case null:
                window.Editor.Options.ShowColumnRulers = false;
                break;
        }
        switch (userSettings.EndOfLineSetting)
        {
            default:
                window.Editor.Options.ShowEndOfLine = userSettings.EndOfLineSetting.Value;
                break;
            case null:
                window.Editor.Options.ShowEndOfLine = false;
                break;
        }
        switch (userSettings.ListViewSetting)
        {
            case null:
                window.FileList.IsVisible = true;
                break;
            case true:
                window.FileList.IsVisible = true;
                break;
            case false:
                window.FileList.IsVisible = false;
                switch (window.Editor.GetValue(Grid.ColumnSpanProperty))
                {
                    case 1:
                        window.Editor.SetValue(Grid.ColumnSpanProperty, 2);
                        break;
                    case 2:
                        window.Editor.SetValue(Grid.ColumnSpanProperty, 1);
                        break;
                }
                break;
        }
        switch (userSettings.LocationBarViewSetting)
        {
            default:
                window.LocationBar.IsVisible = userSettings.LocationBarViewSetting.Value;
                break;
            case null:
                window.LocationBar.IsVisible = true;
                break;
        }
        switch (userSettings.FileBarViewSetting)
        {
            default:
                window.FileBar.IsVisible = userSettings.FileBarViewSetting.Value;
                break;
            case null:
                window.FileBar.IsVisible = true;
                break;
        }
        switch (userSettings.LocationBarSetting)
        {
            default:
                window.LocationBar.SetValue(Grid.RowProperty, userSettings.LocationBarSetting);
                break;
            case null:
                window.LocationBar.SetValue(Grid.RowProperty, 5);
                break;
        }
        switch (userSettings.FileBarSetting)
        {
            default:
                window.FileBar.SetValue(Grid.RowProperty, userSettings.FileBarSetting);
                break;
            case null:
                window.FileBar.SetValue(Grid.RowProperty, 5);
                break;
        }
        // Debug
        switch (userSettings.GridLinesSetting)
        {
            default:
                window.MainGrid.ShowGridLines = userSettings.GridLinesSetting.Value;
                break;
            case null:
                window.MainGrid.ShowGridLines = false;
                break;
        }
    }
    private static string GetTheme(TopLevel topLevel)
    {
        return (topLevel.RequestedThemeVariant).ToString();
    }
    public void RefreshIsChecked(MainWindow window)
    {
        // Refresh theme setting checkbox
        switch (GetTheme(window))
        {
            case "Default":
                window.SystemThemeItem.IsChecked = true;
                window.DarkThemeItem.IsChecked = false;
                window.LightThemeItem.IsChecked = false;
                break;
            case "Dark":
                window.SystemThemeItem.IsChecked = false;
                window.DarkThemeItem.IsChecked = true;
                window.LightThemeItem.IsChecked = false;
                break;
            case "Light":
                window.SystemThemeItem.IsChecked = false;
                window.DarkThemeItem.IsChecked = false;
                window.LightThemeItem.IsChecked = true;
                break;
        }
        // Refresh View settings checkboxes
        switch (window.LocationBar.IsVisible, window.FileBar.IsVisible)
        {
            case (true, true):
                window.ViewStatusBarButton.IsChecked = true;
                break;
            case (false, false):
                window.ViewStatusBarButton.IsChecked = false;
                break;
        }
    }
    public void RefreshList(MainWindow window)
    {
        if (window.FolderPath != string.Empty)
        {
            string[] files = Directory.GetFiles(window.FolderPath);

            window.FileList.Items.Clear();
            foreach (string file in files)
            {
                window.FileList.Items.Add(file);
            }
        }
        else
        {
            Console.WriteLine("No folder selected");
        }
    }
    public void RefreshFileInformation(MainWindow window)
    {
        try
        {
            var textEditor = window.FindControl<TextEditor>("Editor");
            if (window.ActualThemeVariant == ThemeVariant.Default)
            {
                var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
                var textMateInstallation = textEditor.InstallTextMate(registryOptions);
                string languageExtension = registryOptions.GetLanguageByExtension(Path.GetExtension(window.FilePath)).Id;

                textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(languageExtension));
                window.Language = languageExtension.ToUpper();
            }
            else if (window.ActualThemeVariant == ThemeVariant.Light)
            {
                var registryOptions = new RegistryOptions(ThemeName.LightPlus);
                var textMateInstallation = textEditor.InstallTextMate(registryOptions);
                string languageExtension = registryOptions.GetLanguageByExtension(Path.GetExtension(window.FilePath)).Id;

                textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(languageExtension));
                window.Language = languageExtension.ToUpper();
            }
            else if (window.ActualThemeVariant == ThemeVariant.Dark)
            {
                var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
                var textMateInstallation = textEditor.InstallTextMate(registryOptions);
                string languageExtension = registryOptions.GetLanguageByExtension(Path.GetExtension(window.FilePath)).Id;

                textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(languageExtension));
                window.Language = languageExtension.ToUpper();
            }
        }
        catch (Exception)
        {
            window.LanguageStatusText.Text= ("Language: " + "Language Not Found");
            Console.WriteLine("Not a Programming Language/Language Not Supported/ No file selected");
        }
        var fileInfo = new FileInfo(window.FilePath);
        
        var fileExtension = Path.GetExtension(window.FilePath);
        window.Extension = fileExtension;
    }
}