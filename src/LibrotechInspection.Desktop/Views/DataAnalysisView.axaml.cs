using System;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LibrotechInspection.Desktop.ViewModels;
using OxyPlot.Avalonia;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Views;

public partial class DataAnalysisView : ReactiveUserControl<DataAnalysisViewModel>
{
    public DataAnalysisView()
    {
        this.WhenActivated(d =>
        {
            if (ViewModel != null)
            {
                FindShowTemperatureCheckBox.IsEnabled = ViewModel.LinePlotViewModel.HasTemperature;
                FindShowHumidityCheckBox.IsEnabled = ViewModel.LinePlotViewModel.HasHumidity;
                FindShowPressureCheckBox.IsEnabled = ViewModel.LinePlotViewModel.HasPressure;
            }

            d(this.WhenAnyObservable(vm => vm.ViewModel!.LinePlotViewModel.SelectedPointObservable)
                .Select(point => point is not null)
                .Subscribe(pointNotNull =>
                {
                    FindPlotMarkPointFlyoutItem.IsVisible = pointNotNull;
                    FindPlotCreateSeparatorLineFlyoutItem.IsVisible = pointNotNull;
                }));

            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.ModelManager.PlotModel,
                view => view.FindPlotView.Model));
            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.Controller,
                view => view.FindPlotView.Controller));

            d(this.BindCommand(ViewModel, vm => vm.LinePlotViewModel.MarkSelectedPointCommand,
                view => view.FindPlotMarkPointFlyoutItem));
            d(this.BindCommand(ViewModel, vm => vm.LinePlotViewModel.CreateSeparatorLineCommand,
                view => view.FindPlotCreateSeparatorLineFlyoutItem));

            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.DisplayConditions.DisplayTemperature,
                view => view.FindShowTemperatureCheckBox.IsChecked));
            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.DisplayConditions.DisplayHumidity,
                view => view.FindShowHumidityCheckBox.IsChecked));
            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.DisplayConditions.DisplayPressure,
                view => view.FindShowPressureCheckBox.IsChecked));

            d(this.Bind(ViewModel, vm => vm.FileShortSummary,
                view => view.FindShortSummaryContentControl.Content));
            d(this.Bind(ViewModel, vm => vm.SelectedPointInfo,
                view => view.FindSelectedPointInfoContentControl.Content));
        });

        AvaloniaXamlLoader.Load(this);
    }

#region Methods

    private void AlignChartCenter(object sender, RoutedEventArgs e)
    {
        var plotView = FindPlotView;
        foreach (var axis in plotView.Model.Axes) axis.Reset();

        plotView.InvalidatePlot();
    }

#endregion

#region Find Properties

    public PlotView FindPlotView => this.FindControl<PlotView>(nameof(PlotView));
    public MenuItem FindPlotMarkPointFlyoutItem => this.FindControl<MenuItem>(nameof(PlotMarkPointFlyoutItem));

    public MenuItem FindPlotCreateSeparatorLineFlyoutItem =>
        this.FindControl<MenuItem>(nameof(PlotCreateSeparatorLineFlyoutItem));

    public CheckBox FindShowTemperatureCheckBox => this.FindControl<CheckBox>(nameof(ShowTemperatureCheckBox));
    public CheckBox FindShowHumidityCheckBox => this.FindControl<CheckBox>(nameof(ShowHumidityCheckBox));
    public CheckBox FindShowPressureCheckBox => this.FindControl<CheckBox>(nameof(ShowPressureCheckBox));
    public Button FindMarkSelectedPointButton => this.FindControl<Button>(nameof(MarkSelectedPointButton));

    public ContentControl FindSelectedPointInfoContentControl =>
        this.FindControl<ContentControl>(nameof(SelectedPointInfoContentControl));

    public ContentControl FindShortSummaryContentControl =>
        this.FindControl<ContentControl>(nameof(ShortSummaryContentControl));

#endregion
}