using System;
using System.IO;
using System.Text.Json;
using System.Threading;
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
        ScrollBarVisibility[] scrollBarVisibility = [ScrollBarVisibility.Auto, ScrollBarVisibility.Disabled, ScrollBarVisibility.Hidden, ScrollBarVisibility.Visible];
        foreach (var opt in scrollBarVisibility)
        {
            VBarOptions.Items.Add(opt);
            HBarOptions.Items.Add(opt);
        }
        for (int x = 0; x < 64; x++)
        {
            IndentOptions.Items.Add(x);
        }
        var settingsFile= (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/Enviredit/config.json");
        // Deserialize JSON
        if (LocalGetSettingsFile() == null) return;
        using StreamReader reader = new StreamReader(settingsFile);
        var settingsFileContents = reader.ReadToEnd();
        var settingsOutput = JsonSerializer.Deserialize<SettingsClass>(settingsFileContents);
        if (settingsOutput == null) return;
        // Apply settings
        switch (settingsOutput.OptTheme)
        {
            case "Default":
                this.RequestedThemeVariant = ThemeVariant.Default;
                break;
            case "Light":
                this.RequestedThemeVariant = ThemeVariant.Light;
                break;
            case "Dark":
                this.RequestedThemeVariant = ThemeVariant.Dark;
                break;
        }
        Editor.Options.ShowSpaces = settingsOutput.OptShowSpaces;
        Editor.Options.HighlightCurrentLine = settingsOutput.OptHighlightLine;
        Editor.Options.ShowTabs = settingsOutput.OptShowTabs;
        Editor.Options.ConvertTabsToSpaces = settingsOutput.OptConvertTabs;
        MainGrid.ShowGridLines = settingsOutput.OptShowGridLine;
        Editor.Options.IndentationSize = settingsOutput.OptIndentSpacing;
    }

    // Functions to get properties in the view model
    private string LocalGetCurrentFile()
    {
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return String.Empty;
        return vm.GetCurrentFile();
    }
    private string LocalGetCurrentFolder()
    {
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return String.Empty;
        return vm.GetCurrentFolder();
    }
    private string LocalGetSettingsFile()
    {
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return String.Empty;
        return vm.GetSettingsFile();
    }
    private string LocalGetDeletionFile()
    {
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return String.Empty;
        return vm.GetDeletionFile();
    }
    // Functions to set properties in the view model
    private void LocalSetCurrentFile(string input)
    {
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return;
        vm.UpdateCurrentFile(input);
    }
    private void LocalSetCurrentFolder(string input)
    {
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return;
        vm.UpdateCurrentFolder(input);
    }
    private void LocalSetSettingsFile(string input)
    {
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return;
        vm.UpdateSettingsFile(input);
    }
    private void LocalSetDeletionFile(string input)
    {
        var vm = (DataContext as MainWindowViewModel);
        if (vm == null) return;
        vm.UpdateDeletionFile(input);
    }
}