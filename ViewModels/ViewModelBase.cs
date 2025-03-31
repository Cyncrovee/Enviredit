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
        return _currentFile;
    }
    public string GetCurrentFolder()
    {
        return _currentFolder;
    }
    public string GetSettingsFile()
    {
        return _settingsFile;
    }
    public string GetDeletionFile()
    {
        return _deletionFile;
    }
    // Update properties
    public void UpdateCurrentFile(string update)
    {
        _currentFile = update;
    }
    public void UpdateCurrentFolder(string update)
    {
        _currentFolder = update;
    }
    public void UpdateSettingsFile(string update)
    {
        _settingsFile = update;
    }
    public void UpdateDeletionFile(string update)
    {
        _deletionFile = update;
    }
}
