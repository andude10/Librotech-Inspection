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
                view => view.GoToChartButton));
            d(this.BindCommand(ViewModel, vm => vm.GoToDeviceAlarmSettingsCommand,
                view => view.GoToDeviceAlarmSettingsButton));
            d(this.BindCommand(ViewModel, vm => vm.GoToStampsCommand,
                view => view.GoToStampsButton));

            d(this.Bind(ViewModel, vm => vm.RecordHasAlarmSettings,
                view => view.GoToDeviceAlarmSettingsButton.IsVisible));
            d(this.Bind(ViewModel, vm => vm.RecordHasStamps,
                view => view.GoToStampsButton.IsVisible));
            d(this.Bind(ViewModel, vm => vm.RecordIsLoaded,
                view => view.SavePlotMenuItem.IsVisible));

            if (ViewModel != null) d(ViewModel.Router.CurrentViewModel.Subscribe(RoutedViewModelChanged));
        });

        AvaloniaXamlLoader.Load(this);
        InitializeComponent();
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
            GoToChartButton.Classes.Remove("nav-button");
            GoToChartButton.Classes.Add("nav-button-selected");
        }
        else
        {
            GoToChartButton.Classes.Add("nav-button");
            GoToChartButton.Classes.Remove("nav-button-selected");
        }

        if (vmType == typeof(DeviceAlarmSettingsViewModel))
        {
            GoToDeviceAlarmSettingsButton.Classes.Remove("nav-button");
            GoToDeviceAlarmSettingsButton.Classes.Add("nav-button-selected");
        }
        else
        {
            GoToDeviceAlarmSettingsButton.Classes.Add("nav-button");
            GoToDeviceAlarmSettingsButton.Classes.Remove("nav-button-selected");
        }


        if (vmType == typeof(StampsViewModel))
        {
            GoToStampsButton.Classes.Remove("nav-button");
            GoToStampsButton.Classes.Add("nav-button-selected");
        }
        else
        {
            GoToStampsButton.Classes.Add("nav-button");
            GoToStampsButton.Classes.Remove("nav-button-selected");
        }
    }

    private void BindMenuCommands(IRoutableViewModel viewModel)
    {
        if (viewModel is ChartViewModel analysisViewModel)
            SavePlotMenuItem.Command = analysisViewModel.SavePlotAsFileCommand;
    }
}