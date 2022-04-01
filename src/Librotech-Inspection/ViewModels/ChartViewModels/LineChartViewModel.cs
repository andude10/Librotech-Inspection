using System;
using System.Reactive;
using System.Threading.Tasks;
using Librotech_Inspection.Utilities.ChartCustomizers;
using Librotech_Inspection.Utilities.DataDecorators;
using Librotech_Inspection.Utilities.Interactions;
using Librotech_Inspection.Utilities.Parsers.ChartDataParsers;
using Librotech_Inspection.Utilities.Parsers.ChartDataParsers.CsvFile;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReactiveUI;
using Splat;

namespace Librotech_Inspection.ViewModels.ChartViewModels;

/// <summary>
///     LineChartViewModel represents the chart, is responsible for plotting the chart
/// </summary>
public sealed class LineChartViewModel : ChartViewModel
{
    private PlotModel? _plotModel;
    private ChartDataParser _chartDataParser;

    public LineChartViewModel(ChartCustomizer chartCustomizer)
    {
        _chartCustomizer = chartCustomizer;
        _plotModel = new PlotModel();
        _chartDataParser = Locator.Current.GetService<ChartDataParser>() 
                           ?? throw new InvalidOperationException();

        // TODO: the ReactiveUI documentation says not to use
        // .Subscribe() for anything more serious than logging.
        // Will need to rewrite.
        this.WhenAnyValue(vm => vm.ShowTemperature)
            .Subscribe(x => CreateModel());
        this.WhenAnyValue(vm => vm.ShowHumidity)
            .Subscribe(x => CreateModel());
        this.WhenAnyValue(vm => vm.ShowPressure)
            .Subscribe(x => CreateModel());
    }

    public override PlotModel? PlotModel
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

    private LineSeries Temperature { get; } = new() {Tag = ChartElementTags.LineSeriesTemperature};
    private LineSeries Humidity { get; } = new() {Tag = ChartElementTags.LineSeriesHumidity};
    private LineSeries Pressure { get; } = new() {Tag = ChartElementTags.LineSeriesPressure};

    private LinearAxis TemperatureYAxis { get; } = new() {Tag = ChartElementTags.TemperatureYAxis};
    private LinearAxis HumidityYAxis { get; } = new() {Tag = ChartElementTags.HumidityYAxis};
    private LinearAxis PressureYAxis { get; } = new() {Tag = ChartElementTags.PressureYAxis};

    private DateTimeAxis XAxis { get; } = new() {Tag = ChartElementTags.DateTimeAxis};

#endregion

#region Methods

    public override async Task BuildAsync(string chartData)
    {
        // Clear old chartData
        Temperature.Points.Clear();
        Humidity.Points.Clear();
        Pressure.Points.Clear();

        // Load LineSeries
        if (ShowTemperature)
            await foreach (var point in _chartDataParser.ParseTemperatureAsync(chartData))
                Temperature.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));

        if (ShowHumidity)
            await foreach (var point in _chartDataParser.ParseHumidityAsync(chartData))
                Humidity.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));

        if (ShowPressure)
            await foreach (var point in _chartDataParser.ParsePressureAsync(chartData))
                Pressure.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));

        CreateModel();
    }

    public override void CreateModel()
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