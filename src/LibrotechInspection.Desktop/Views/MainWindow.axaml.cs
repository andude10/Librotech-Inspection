using System;
using Avalonia;
using Avalonia.Controls;
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

        if (vmType == typeof(ChartViewModel))
        {
            FindGoToChartButton.Classes.Remove("nav-button");
            FindGoToChartButton.Classes.Add("nav-button-selected");
        }
        else
        {
            FindGoToChartButton.Classes.Add("nav-button");
            FindGoToChartButton.Classes.Remove("nav-button-selected");
        }

        if (vmType == typeof(ConfigurationViewModel))
        {
            FindGoToConfigurationButton.Classes.Remove("nav-button");
            FindGoToConfigurationButton.Classes.Add("nav-button-selected");
        }
        else
        {
            FindGoToConfigurationButton.Classes.Add("nav-button");
            FindGoToConfigurationButton.Classes.Remove("nav-button-selected");
        }
    }

    private void BindMenuCommands(IRoutableViewModel viewModel)
    {
        if (viewModel is ChartViewModel analysisViewModel)
            FindSavePlotMenuItem.Command = analysisViewModel.SavePlotAsFileCommand;
    }

#region Find Properties

    public Button FindGoToChartButton => this.FindControl<Button>(nameof(GoToChartButton));
    public Button FindGoToConfigurationButton => this.FindControl<Button>(nameof(GoToConfigurationButton));
    public MenuItem FindSavePlotMenuItem => this.FindControl<MenuItem>(nameof(SavePlotMenuItem));

#endregion
}