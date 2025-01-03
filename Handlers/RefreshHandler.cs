using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace Enviredit;

public partial class MainWindow : Window
{
    private void RefreshSettings()
    {
        var jsonString = File.ReadAllText(SettingsFile);
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
                List<FontFamily> notFoundFonts = new List<FontFamily>();
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
                        notFoundFonts.Add(fontFamily);
                    }
                }
                Console.WriteLine("Not found " + notFoundFonts.Count + " fonts");
                break;
        }
        // Refresh scrollbar settings
        try
        {
            Editor.VerticalScrollBarVisibility = userSettings.VScrollViewSetting;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        try
        {
            Editor.HorizontalScrollBarVisibility = userSettings.HScrollViewSetting;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        // Refresh MenuBar settings
        // Edit
        switch (userSettings.RectangularEditSetting)
        {
            default:
                Editor.Options.EnableRectangularSelection = userSettings.RectangularEditSetting.Value;
                break;
            case null:
                Editor.Options.EnableRectangularSelection = true;
                break;
        }
        // View
        switch (userSettings.ScrollBelowDocumentSetting)
        {
            default:
                Editor.Options.AllowScrollBelowDocument = userSettings.ScrollBelowDocumentSetting.Value;
                break;
            case null:
                Editor.Options.AllowScrollBelowDocument = true;
                break;
                
        }
        switch (userSettings.RowHighlightSetting)
        {
            default:
                Editor.Options.HighlightCurrentLine = userSettings.RowHighlightSetting.Value;
                break;
            case null:
                Editor.Options.HighlightCurrentLine = true;
                break;
        }
        switch (userSettings.SpacesEditorSetting)
        {
            default:
                Editor.Options.ShowSpaces = userSettings.SpacesEditorSetting.Value;
                break;
            case null:
                Editor.Options.ShowSpaces = false;
                break;
        }
        switch (userSettings.TabSpacesEditorSetting)
        {
            default:
                Editor.Options.ShowTabs = userSettings.TabSpacesEditorSetting.Value;
                break;
            case null:
                Editor.Options.ShowTabs = false;
                break;
        }
        switch (userSettings.ColumnRulerSetting)
        {
            default:
                Editor.Options.ShowColumnRulers = userSettings.ColumnRulerSetting.Value;
                break;
            case null:
                Editor.Options.ShowColumnRulers = false;
                break;
        }
        switch (userSettings.EndOfLineSetting)
        {
            default:
                Editor.Options.ShowEndOfLine = userSettings.EndOfLineSetting.Value;
                break;
            case null:
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
        switch (userSettings.LocationBarViewSetting)
        {
            default:
                LocationBar.IsVisible = userSettings.LocationBarViewSetting.Value;
                break;
            case null:
                LocationBar.IsVisible = true;
                break;
        }
        switch (userSettings.FileBarViewSetting)
        {
            default:
                FileBar.IsVisible = userSettings.FileBarViewSetting.Value;
                break;
            case null:
                FileBar.IsVisible = true;
                break;
        }
        switch (userSettings.LocationBarSetting)
        {
            default:
                LocationBar.SetValue(Grid.RowProperty, userSettings.LocationBarSetting);
                break;
            case null:
                LocationBar.SetValue(Grid.RowProperty, 5);
                break;
        }
        switch (userSettings.FileBarSetting)
        {
            default:
                FileBar.SetValue(Grid.RowProperty, userSettings.FileBarSetting);
                break;
            case null:
                FileBar.SetValue(Grid.RowProperty, 5);
                break;
        }
        // Debug
        switch (userSettings.GridLinesSetting)
        {
            default:
                MainGrid.ShowGridLines = userSettings.GridLinesSetting.Value;
                break;
            case null:
                MainGrid.ShowGridLines = false;
                break;
        }
    }
    private void RefreshIsChecked()
    {
        // Refresh theme setting checkbox
        switch (GetTheme(this))
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
        // Refresh View settings checkboxes
        switch (LocationBar.IsVisible, FileBar.IsVisible)
        {
            case (true, true):
                ViewStatusBarButton.IsChecked = true;
                break;
            case (false, false):
                ViewStatusBarButton.IsChecked = false;
                break;
        }
    }
    private void RefreshList()
    {
        if (FolderPath != String.Empty)
        {
            string[] files = Directory.GetFiles(FolderPath);

            FileList.Items.Clear();
            foreach (string file in files)
            {
                FileList.Items.Add(file);
            }
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
            MainInstallation.Dispose();
            if (ActualThemeVariant == ThemeVariant.Default)
            {
                _mainRegistryOptions.LoadTheme(ThemeName.DarkPlus);
            }
            else if (ActualThemeVariant == ThemeVariant.Light)
            {
                _mainRegistryOptions.LoadTheme(ThemeName.LightPlus);
            }
            else if (ActualThemeVariant == ThemeVariant.Dark)
            {
                _mainRegistryOptions.LoadTheme(ThemeName.DarkPlus);
            }
            if (FilePath != String.Empty)
            {
                Console.WriteLine("Main Grammar");
                string languageExtension = _mainRegistryOptions.GetLanguageByExtension(Path.GetExtension(FilePath)).Id;
                MainInstallation = Editor.InstallTextMate(_mainRegistryOptions);
                MainInstallation.SetGrammar(_mainRegistryOptions.GetScopeByLanguageId(languageExtension));
                Language = languageExtension.ToUpper();
            }
            else if (FilePath == String.Empty)
            {
                Console.WriteLine("Default Grammar");
                MainInstallation = Editor.InstallTextMate(_mainRegistryOptions);
                MainInstallation.SetGrammar(_mainRegistryOptions.GetScopeByLanguageId(".txt"));
                Language = "Language not found";
            }
        }
        catch (NullReferenceException)
        {
            Language = "Language Not Found";
        }

        Extension = Path.GetExtension(FilePath);
    }
}