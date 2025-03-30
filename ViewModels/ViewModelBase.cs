using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Enviredit.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    public string _currentFile = String.Empty;
    public void UpdateCurrentFile(string update)
    {
        _currentFile = update;
    }
    public string GetCurrentFile()
    {
        return _currentFile;
    }
    [ObservableProperty]
    public string _currentFolder = String.Empty;
    public void UpdateCurrentFolder(string update)
    {
        _currentFolder = update;
    }
    public string GetCurrentFolder()
    {
        return _currentFolder;
    }
    [ObservableProperty]
    public string _settingsFile = String.Empty;
    public void UpdateSettingsFile(string update)
    {
        _settingsFile = update;
    }
    public string GetSettingsFile()
    {
        return _settingsFile;
    }
}
