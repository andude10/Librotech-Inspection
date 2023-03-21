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
            d(this.Bind(ViewModel, vm => vm.WindowTitle,
                view => view.Title));

            d(this.BindCommand(ViewModel, vm => vm.GoToChartCommand,
                view => view.FindGoToChartButton));
            d(this.BindCommand(ViewModel, vm => vm.GoToDeviceAlarmSettingsCommand,
                view => view.FindGoToDeviceAlarmSettingsButton));
            d(this.BindCommand(ViewModel, vm => vm.GoToStampsCommand,
                view => view.FindGoToStampsButton));

            d(this.Bind(ViewModel, vm => vm.RecordHasAlarmSettings,
                view => view.FindGoToDeviceAlarmSettingsButton.IsVisible));
            d(this.Bind(ViewModel, vm => vm.RecordHasStamps,
                view => view.FindGoToStampsButton.IsVisible));
            d(this.Bind(ViewModel, vm => vm.RecordIsLoaded,
                view => view.FindSavePlotMenuItem.IsVisible));

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

        // TODO: remove this shame
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

        if (vmType == typeof(DeviceAlarmSettingsViewModel))
        {
            FindGoToDeviceAlarmSettingsButton.Classes.Remove("nav-button");
            FindGoToDeviceAlarmSettingsButton.Classes.Add("nav-button-selected");
        }
        else
        {
            FindGoToDeviceAlarmSettingsButton.Classes.Add("nav-button");
            FindGoToDeviceAlarmSettingsButton.Classes.Remove("nav-button-selected");
        }


        if (vmType == typeof(StampsViewModel))
        {
            FindGoToStampsButton.Classes.Remove("nav-button");
            FindGoToStampsButton.Classes.Add("nav-button-selected");
        }
        else
        {
            FindGoToStampsButton.Classes.Add("nav-button");
            FindGoToStampsButton.Classes.Remove("nav-button-selected");
        }
    }

    private void BindMenuCommands(IRoutableViewModel viewModel)
    {
        if (viewModel is ChartViewModel analysisViewModel)
            FindSavePlotMenuItem.Command = analysisViewModel.SavePlotAsFileCommand;
    }

    #region Find Properties

    public Button FindGoToStampsButton => this.FindControl<Button>(nameof(GoToStampsButton));
    public Button FindGoToChartButton => this.FindControl<Button>(nameof(GoToChartButton));
    public Button FindGoToDeviceAlarmSettingsButton => this.FindControl<Button>(nameof(GoToDeviceAlarmSettingsButton));
    public MenuItem FindSavePlotMenuItem => this.FindControl<MenuItem>(nameof(SavePlotMenuItem));

    #endregion
}