using System;
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
        var jsonString = File.ReadAllText(window._settingsFile);
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
                window.Editor.Options.EnableRectangularSelection = true;
                break;
            case true:
                window.Editor.Options.EnableRectangularSelection = true;
                break;
            case false:
                window.Editor.Options.EnableRectangularSelection = false;
                break;
        }
        // View
        switch (userSettings.ScrollBelowDocumentSetting)
        {
            case null:
                window.Editor.Options.AllowScrollBelowDocument = true;
                break;
            case true:
                window.Editor.Options.AllowScrollBelowDocument = true;
                break;
            case false:
                window.Editor.Options.AllowScrollBelowDocument = false;
                break;
        }
        switch (userSettings.RowHighlightSetting)
        {
            case null:
                window.Editor.Options.HighlightCurrentLine = true;
                break;
            case true:
                window.Editor.Options.HighlightCurrentLine = true;
                break;
            case false:
                window.Editor.Options.HighlightCurrentLine = false;
                break;
        }
        switch (userSettings.SpacesEditorSetting)
        {
            case null:
                window.Editor.Options.ShowSpaces = false;
                break;
            case true:
                window.Editor.Options.ShowSpaces = true;
                break;
            case false:
                window.Editor.Options.ShowSpaces = false;
                break;
        }
        switch (userSettings.TabSpacesEditorSetting)
        {
            case null:
                window.Editor.Options.ShowTabs = false;
                break;
            case true:
                window.Editor.Options.ShowTabs = true;
                break;
            case false:
                window.Editor.Options.ShowTabs = false;
                break;
        }
        switch (userSettings.ColumnRulerSetting)
        {
            case null:
                window.Editor.Options.ShowColumnRulers = false;
                break;
            case true:
                window.Editor.Options.ShowColumnRulers = true;
                break;
            case false:
                window.Editor.Options.ShowColumnRulers = false;
                break;
        }
        switch (userSettings.EndOfLineSetting)
        {
            case null:
                window.Editor.Options.ShowEndOfLine = false;
                break;
            case true:
                window.Editor.Options.ShowEndOfLine = true;
                break;
            case false:
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
            case null:
                window.LocationBar.IsVisible = true;
                break;
            case true:
                window.LocationBar.IsVisible = true;
                break;
            case false:
                window.LocationBar.IsVisible = false;
                break;
        }
        switch (userSettings.FileBarViewSetting)
        {
            case null:
                window.FileBar.IsVisible = true;
                break;
            case true:
                window.FileBar.IsVisible = true;
                break;
            case false:
                window.FileBar.IsVisible = false;
                break;
        }
        switch (userSettings.LocationBarSetting)
        {
            case null:
                window.LocationBar.SetValue(Grid.RowProperty, 5);
                break;
            case 5:
                window.LocationBar.SetValue(Grid.RowProperty, 5);
                break;
            case 3:
                window.LocationBar.SetValue(Grid.RowProperty, 3);
                break;
        }
        switch (userSettings.FileBarSetting)
        {
            case null:
                window.FileBar.SetValue(Grid.RowProperty, 5);
                break;
            case 5:
                window.FileBar.SetValue(Grid.RowProperty, 5);
                break;
            case 3:
                window.FileBar.SetValue(Grid.RowProperty, 3);
                break;
        }
        // Debug
        switch (userSettings.GridLinesSetting)
        {
            case null:
                window.MainGrid.ShowGridLines = false;
                break;
            case true:
                window.MainGrid.ShowGridLines = true;
                break;
            case false:
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
        if (window._folderPath != string.Empty)
        {
            string[] files = Directory.GetFiles(window._folderPath);

            window.FileList.Items.Clear();
            foreach (string file in files)
            {
                window.FileList.Items.Add(file);
            }
            window.FolderPathBlock.Text = "Current File: " + window._folderPath;
        }
        else
        {
            Console.WriteLine("No folder selected");
        }
    }
    public void LoadFromList(MainWindow window)
    {
        if (window.FileList.SelectedItem != null)
        {
            var selectedFile = window.FileList.SelectedItem.ToString();
            window._filePath = selectedFile;
            window.Editor.Text = string.Empty;
            window.Editor.Clear();

            try
            {
                using StreamReader reader = new(selectedFile);
                var text = reader.ReadToEnd();
                window.Editor.Text = text;
                reader.Close();
                window.FilePathBlock.Text = "Current File: " + selectedFile;
                window.settingsHandler.SaveSettings(window);
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
        RefreshFileInformation(window);
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
                string languageExtension = registryOptions.GetLanguageByExtension(Path.GetExtension(window._filePath)).Id;

                textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(languageExtension));
                window.LanguageStatusText.Text= ("Language: " + languageExtension.ToUpper());
            }
            else if (window.ActualThemeVariant == ThemeVariant.Light)
            {
                var registryOptions = new RegistryOptions(ThemeName.LightPlus);
                var textMateInstallation = textEditor.InstallTextMate(registryOptions);
                string languageExtension = registryOptions.GetLanguageByExtension(Path.GetExtension(window._filePath)).Id;

                textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(languageExtension));
                window.LanguageStatusText.Text= ("Language: " + languageExtension.ToUpper());
            }
            else if (window.ActualThemeVariant == ThemeVariant.Dark)
            {
                var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
                var textMateInstallation = textEditor.InstallTextMate(registryOptions);
                string languageExtension = registryOptions.GetLanguageByExtension(Path.GetExtension(window._filePath)).Id;

                textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(languageExtension));
                window.LanguageStatusText.Text= ("Language: " + languageExtension.ToUpper());
            }
        }
        catch (Exception)
        {
            window.LanguageStatusText.Text= ("Language: " + "Language Not Found");
            Console.WriteLine("Not a Programming Language/Language Not Supported/ No file selected");
        }
        var fileInfo = new FileInfo(window._filePath);
        window.FileSizeText.Text = "File Size: " + fileInfo.Length.ToString() + "b" + " | ";
        var fileExtension = Path.GetExtension(window._filePath);
        window.FileExtensionText.Text = ("File Extension: " + fileExtension + " | ");
    }
}