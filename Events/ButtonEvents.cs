using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace Enviredit;

public partial class MainWindow : Window
{
    private async void OpenFileButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await _fileHandler.OpenFileDialog(this);
        _fileHandler.LoadFile(this);
        _mainRefreshHandler.RefreshFileInformation(this);
    }
    private async void OpenFolderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await _folderHandler.OpenFolderDialog(this);
        _mainRefreshHandler.RefreshList(this);
        MainSettingsHandler.SaveSettings(this);
    }
    private void SystemThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemThemeItem.IsChecked = true;
        LightThemeItem.IsChecked = false;
        DarkThemeItem.IsChecked = false;

        RequestedThemeVariant = ThemeVariant.Default;
        _mainRefreshHandler.RefreshFileInformation(this);
        MainSettingsHandler.SaveSettings(this);;
    }
    private void LightThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemThemeItem.IsChecked = false;
        LightThemeItem.IsChecked = true;
        DarkThemeItem.IsChecked = false;

        RequestedThemeVariant = ThemeVariant.Light;
        _mainRefreshHandler.RefreshFileInformation(this);
        MainSettingsHandler.SaveSettings(this);;
    }
    private void DarkThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemThemeItem.IsChecked = false;
        LightThemeItem.IsChecked = false;
        DarkThemeItem.IsChecked = true;

        RequestedThemeVariant = ThemeVariant.Dark;
        _mainRefreshHandler.RefreshFileInformation(this);
        MainSettingsHandler.SaveSettings(this);;
    }
    private void IndentationSizeComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IndentationSizeComboBox.SelectedItem == null) return;
        Editor.Options.IndentationSize = IndentationSizeComboBox.SelectedIndex + 1;
        Console.WriteLine(Editor.Options.IndentationSize);
    }
}