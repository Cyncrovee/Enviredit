using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Enviredit.Views;

public partial class MainWindow : Window
{
    public string CurrentFile { get; set; } = string.Empty;
    public string CurrentFolder { get; set; } = string.Empty;
    public MainWindow()
    {
        InitializeComponent();

        Editor.Options.ShowSpaces = true;
        Editor.Options.HighlightCurrentLine = true;
        Editor.Options.ConvertTabsToSpaces = true;

        ScrollBarVisibility[] scrollBarVisibility = [ScrollBarVisibility.Auto, ScrollBarVisibility.Disabled, ScrollBarVisibility.Hidden, ScrollBarVisibility.Visible];

        foreach (var opt in scrollBarVisibility)
        {
            VBarOptions.Items.Add(opt);
            HBarOptions.Items.Add(opt);
        }
    }
}