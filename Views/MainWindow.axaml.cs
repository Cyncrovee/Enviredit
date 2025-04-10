using System;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using Enviredit.ViewModels;

namespace Enviredit.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Add contents to combo boxes
        ScrollBarVisibility[] scrollBarVisibility =
        [
            ScrollBarVisibility.Auto, ScrollBarVisibility.Disabled, ScrollBarVisibility.Hidden,
            ScrollBarVisibility.Visible
        ];
        foreach (var opt in scrollBarVisibility)
        {
            VBarOptions.Items.Add(opt);
            HBarOptions.Items.Add(opt);
        }

        for (var x = 0; x < 64; x++) IndentOptions.Items.Add(x);
        var settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                           "/.config/Enviredit/config.json";
        // This would normally be handled by the LoadSettings() function in the settings module,
        // However that doesn't work here due to being unable to access the view model
        // Deserialize JSON
        using var reader = new StreamReader(settingsFile);
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

    // Functions to get properties in the view model
    // These exist to avoid having to repeatedly make a variable to reference the view model
    private string LocalGetCurrentFile()
    {
        return DataContext is not MainWindowViewModel vm ? string.Empty : vm.GetCurrentFile();
    }

    private string LocalGetCurrentFolder()
    {
        return DataContext is not MainWindowViewModel vm ? string.Empty : vm.GetCurrentFolder();
    }

    private string LocalGetSettingsFile()
    {
        return DataContext is not MainWindowViewModel vm ? string.Empty : vm.GetSettingsFile();
    }

    private string LocalGetDeletionFile()
    {
        return DataContext is not MainWindowViewModel vm ? string.Empty : vm.GetDeletionFile();
    }

    // Functions to set properties in the view model
    private void LocalSetCurrentFile(string input)
    {
        var vm = DataContext as MainWindowViewModel;
        vm?.UpdateCurrentFile(input);
    }

    private void LocalSetCurrentFolder(string input)
    {
        var vm = DataContext as MainWindowViewModel;
        vm?.UpdateCurrentFolder(input);
    }

    private void LocalSetSettingsFile(string input)
    {
        var vm = DataContext as MainWindowViewModel;
        vm?.UpdateSettingsFile(input);
    }

    private void LocalSetDeletionFile(string input)
    {
        var vm = DataContext as MainWindowViewModel;
        vm?.UpdateDeletionFile(input);
    }
}