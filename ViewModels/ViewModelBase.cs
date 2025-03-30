using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Enviredit.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    public string _currentFile = String.Empty;
    [ObservableProperty]
    public string _currentFolder = String.Empty;
}
