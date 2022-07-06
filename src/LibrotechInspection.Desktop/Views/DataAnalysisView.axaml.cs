using System;
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
                if (!ViewModel.ChartViewModel.HasTemperature) FindShowTemperatureCheckBox.IsEnabled = false;
                if (!ViewModel.ChartViewModel.HasHumidity) FindShowHumidityCheckBox.IsEnabled = false;
                if (!ViewModel.ChartViewModel.HasPressure) FindShowPressureCheckBox.IsEnabled = false;
            }

            d(this.Bind(ViewModel, vm => vm.ChartViewModel.PlotModel,
                view => view.FindPlotView.Model));

            d(this.Bind(ViewModel, vm => vm.ChartViewModel.ShowTemperature,
                view => view.FindShowTemperatureCheckBox.IsChecked));
            d(this.Bind(ViewModel, vm => vm.ChartViewModel.ShowHumidity,
                view => view.FindShowHumidityCheckBox.IsChecked));
            d(this.Bind(ViewModel, vm => vm.ChartViewModel.ShowPressure,
                view => view.FindShowPressureCheckBox.IsChecked));

            d(this.Bind(ViewModel, vm => vm.FileShortSummary,
                view => view.FindShortSummaryContentControl.Content));

            if (ViewModel != null) d(ViewModel.ChartViewModel.PlotModelUpdate.Subscribe(model => UpdatePlotView()));
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

    private void UpdatePlotView()
    {
        FindPlotView.InvalidatePlot();
    }

#endregion

#region Find Properties

    public PlotView FindPlotView => this.FindControl<PlotView>(nameof(PlotView));
    public CheckBox FindShowTemperatureCheckBox => this.FindControl<CheckBox>(nameof(ShowTemperatureCheckBox));
    public CheckBox FindShowHumidityCheckBox => this.FindControl<CheckBox>(nameof(ShowHumidityCheckBox));
    public CheckBox FindShowPressureCheckBox => this.FindControl<CheckBox>(nameof(ShowPressureCheckBox));

    public ContentControl FindShortSummaryContentControl =>
        this.FindControl<ContentControl>(nameof(ShortSummaryContentControl));

#endregion
}