using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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
    }

    // Functions to get/set properties in the view model
    private string LocalGetCurrentFile()
    {
        var vm = (DataContext as MainWindowViewModel);
        return vm.GetCurrentFile();
    }
    private string LocalGetCurrentFolder()
    {
        var vm = (DataContext as MainWindowViewModel);
        return vm.GetCurrentFolder();
    }
    private string LocalGetSettingsFile()
    {
        var vm = (DataContext as MainWindowViewModel);
        return vm.GetSettingsFile();
    }
    private void LocalSetCurrentFile(string input)
    {
        var vm = (DataContext as MainWindowViewModel);
        vm.UpdateCurrentFile(input);
    }
    private void LocalSetCurrentFolder(string input)
    {
        var vm = (DataContext as MainWindowViewModel);
        vm.UpdateCurrentFolder(input);
    }
    private void LocalSetSettingsFile(string input)
    {
        var vm = (DataContext as MainWindowViewModel);
        vm.UpdateSettingsFile(input);
    }
}