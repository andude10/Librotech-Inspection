using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LibrotechInspection.Desktop.ViewModels;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        this.WhenActivated(d => 
        {
            if (ViewModel != null) d(ViewModel.Router.CurrentViewModel.Subscribe(RoutedViewModelChanged));
        });

#if DEBUG
        this.AttachDevTools();
#endif

        AvaloniaXamlLoader.Load(this);
    }

    private void RoutedViewModelChanged(IRoutableViewModel? viewModel)
    {
        if (viewModel == null) return;
        
        HighlightNavigationButton(viewModel);
        BindMenuCommands(viewModel);
    }

    private void HighlightNavigationButton(IRoutableViewModel viewModel)
    {
        var vmType = viewModel?.GetType();

        if (vmType == typeof(DataAnalysisViewModel))
        {
            FindGoToDataAnalysisButton.Classes.Remove("nav-button");
            FindGoToDataAnalysisButton.Classes.Add("nav-button-selected");
        }
        else
        {
            FindGoToDataAnalysisButton.Classes.Add("nav-button");
            FindGoToDataAnalysisButton.Classes.Remove("nav-button-selected");
        }

        if (vmType == typeof(ConfigurationViewModel))
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

    private void BindMenuCommands(IRoutableViewModel viewModel)
    {
        if (viewModel is DataAnalysisViewModel analysisViewModel)
        {
            FindSavePlotMenuItem.Command = analysisViewModel.SavePlotAsFileCommand;
        }
    }

#region Find Properties

    public Button FindGoToDataAnalysisButton => this.FindControl<Button>(nameof(GoToDataAnalysisButton));
    public Button FindGoToLoggerConfigurationButton => this.FindControl<Button>(nameof(GoToLoggerConfigurationButton));
    public MenuItem FindSavePlotMenuItem => this.FindControl<MenuItem>(nameof(SavePlotMenuItem));

#endregion
}