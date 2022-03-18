using System;
using System.Reactive;
using System.Threading.Tasks;
using Librotech_Inspection.Utilities;
using Librotech_Inspection.Utilities.ChartCustomizers;
using Librotech_Inspection.Utilities.Interactions;
using Librotech_Inspection.Utilities.Parsers.ChartDataParsers;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReactiveUI;

namespace Librotech_Inspection.ViewModels.ChartViewModels;

/// <summary>
///     LineChartViewModel represents the chart, is responsible for plotting the chart
/// </summary>
public sealed class LineChartViewModel : ChartViewModel
{
    private PlotModel _plotModel;

    public LineChartViewModel(ChartCustomizer chartCustomizer)
    {
        _chartCustomizer = chartCustomizer;
        _plotModel = new PlotModel();

        this.WhenAnyValue(vm => vm.ShowTemperature)
            .Subscribe(x => CreateModel());
        this.WhenAnyValue(vm => vm.ShowHumidity)
            .Subscribe(x => CreateModel());
        this.WhenAnyValue(vm => vm.ShowPressure)
            .Subscribe(x => CreateModel());
    }

    public override PlotModel PlotModel
    {
        get => _plotModel;
        set
        {
            this.RaiseAndSetIfChanged(ref _plotModel, value);
            PlotViewInteractions.UpdatePlotView.Handle(Unit.Default);
        }
    }

#region Private Properties

    private readonly ChartCustomizer _chartCustomizer;

    private LineSeries Temperature { get; } = new() { Tag = ChartElementTags.LineSeriesTemperature };
    private LineSeries Humidity { get; } = new() { Tag = ChartElementTags.LineSeriesHumidity };
    private LineSeries Pressure { get; } = new() { Tag = ChartElementTags.LineSeriesPressure };

    private LinearAxis TemperatureYAxis { get; } = new() { Tag = ChartElementTags.TemperatureYAxis };
    private LinearAxis HumidityYAxis { get; } = new() { Tag = ChartElementTags.HumidityYAxis };
    private LinearAxis PressureYAxis { get; } = new() { Tag = ChartElementTags.PressureYAxis };

    private DateTimeAxis XAxis { get; } = new() { Tag = ChartElementTags.DateTimeAxis };

#endregion

#region Methods

    public override async Task BuildAsync(string data)
    {
        // Clear old data
        Temperature.Points.Clear();
        Humidity.Points.Clear();
        Pressure.Points.Clear();

        // Load LineSeries
        if (ShowTemperature)
            await foreach (var point in LineChartDataParser.ParseTemperatureAsync(data))
                Temperature.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));

        if (ShowHumidity)
            await foreach (var point in LineChartDataParser.ParseHumidityAsync(data))
                Humidity.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));

        if (ShowPressure)
            await foreach (var point in LineChartDataParser.ParsePressureAsync(data))
                Pressure.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));

        CreateModel();
    }

    private void CreateModel()
    {
        PlotModel.Axes.Clear();
        PlotModel.Series.Clear();

        var model = new PlotModel();

        if (ShowTemperature & (Temperature.Points.Count != 0))
        {
            model.Axes.Add(TemperatureYAxis);
            model.Series.Add(Temperature);
        }

        if (ShowHumidity & (Humidity.Points.Count != 0))
        {
            model.Axes.Add(HumidityYAxis);
            model.Series.Add(Humidity);
        }

        if (ShowPressure & (Pressure.Points.Count != 0))
        {
            model.Axes.Add(PressureYAxis);
            model.Series.Add(Pressure);
        }

        model.Axes.Add(XAxis);

        _chartCustomizer.Customize(model);

        PlotModel = model;
    }

#endregion
}