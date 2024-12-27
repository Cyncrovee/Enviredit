using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace Enviredit;

public partial class MainWindow : Window
{
    private async void OpenFileButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await OpenFileDialog();
        LoadFile();
        RefreshFileInformation();
    }
    private async void OpenFolderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await OpenFolderDialog();
        RefreshList();
        SaveSettings();
    }
    private void SystemThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemThemeItem.IsChecked = true;
        LightThemeItem.IsChecked = false;
        DarkThemeItem.IsChecked = false;

        RequestedThemeVariant = ThemeVariant.Default;
        RefreshFileInformation();
        SaveSettings();
    }
    private void LightThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemThemeItem.IsChecked = false;
        LightThemeItem.IsChecked = true;
        DarkThemeItem.IsChecked = false;

        RequestedThemeVariant = ThemeVariant.Light;
        RefreshFileInformation();
        SaveSettings();
    }
    private void DarkThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        SystemThemeItem.IsChecked = false;
        LightThemeItem.IsChecked = false;
        DarkThemeItem.IsChecked = true;

        RequestedThemeVariant = ThemeVariant.Dark;
        RefreshFileInformation();
        SaveSettings();
    }
    private void IndentationSizeComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IndentationSizeComboBox.SelectedItem == null) return;
        Editor.Options.IndentationSize = IndentationSizeComboBox.SelectedIndex + 1;
        Console.WriteLine(Editor.Options.IndentationSize);
    }
}