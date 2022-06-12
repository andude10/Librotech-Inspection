using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Reactive;
using Avalonia.ReactiveUI;
using LibrotechInspection.Desktop.ViewModels;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        this.WhenActivated(d => { });
        
        AvaloniaXamlLoader.Load(this);
    }

    private void HighlightNavigationButton(object sender, RoutedEventArgs e)
    {
        sender = (Button) sender;
        
        if (Equals(sender, FindGoToDataAnalysisButton))
        {
            FindGoToDataAnalysisButton.Classes.Remove("nav-button");
            FindGoToDataAnalysisButton.Classes.Add("nav-button-selected");
        }
        else
        {
            FindGoToDataAnalysisButton.Classes.Add("nav-button");
            FindGoToDataAnalysisButton.Classes.Remove("nav-button-selected");
        }
        
        if (Equals(sender, FindGoToLoggerConfigurationButton))
        {
            FindGoToLoggerConfigurationButton.Classes.Remove("nav-button");
            FindGoToLoggerConfigurationButton.Classes.Add("nav-button-selected");
        }
        else
        {
            FindGoToLoggerConfigurationButton.Classes.Add("nav-button");
            FindGoToLoggerConfigurationButton.Classes.Remove("nav-button-selected");
        }
    }
    
#region Find Properties

    public Button FindGoToDataAnalysisButton => this.FindControl<Button>(nameof(GoToDataAnalysisButton));
    public Button FindGoToLoggerConfigurationButton => this.FindControl<Button>(nameof(GoToLoggerConfigurationButton));

#endregion
}