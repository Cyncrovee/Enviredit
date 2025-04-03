using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Enviredit.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    // Set properties
    [ObservableProperty]
    private string _currentFile = String.Empty;
    [ObservableProperty]
    private string _currentFolder = String.Empty;
    [ObservableProperty]
    private string _settingsFile = String.Empty;
    [ObservableProperty]
    private string _deletionFile = String.Empty;
    // Get properties
    public string GetCurrentFile()
    {
        return CurrentFile;
    }
    public string GetCurrentFolder()
    {
        return CurrentFolder;
    }
    public string GetSettingsFile()
    {
        return SettingsFile;
    }
    public string GetDeletionFile()
    {
        return DeletionFile;
    }
    // Update properties
    public void UpdateCurrentFile(string update)
    {
        CurrentFile = update;
    }
    public void UpdateCurrentFolder(string update)
    {
        CurrentFolder = update;
    }
    public void UpdateSettingsFile(string update)
    {
        SettingsFile = update;
    }
    public void UpdateDeletionFile(string update)
    {
        DeletionFile = update;
    }
}
