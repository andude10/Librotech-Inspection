using System.Reactive;
using System.Threading.Tasks;
using Librotech_Inspection.Utilities.Parsers.ChartDataParsers;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReactiveUI;

namespace Librotech_Inspection.ViewModels.ChartViewModels;

/// <summary>
///     LineSeriesViewModel represents the chart, is responsible for plotting the chart
/// </summary>
public class LineSeriesViewModel : ChartViewModel
{
    private PlotModel _plotModel;

    public override PlotModel PlotModel
    {
        get => _plotModel;
        set
        {
            this.RaiseAndSetIfChanged(ref _plotModel, value);
            UpdatePlotView.Handle(Unit.Default);
        }
    }

    private LineSeries Temperature { get; } = new();
    private LineSeries Humidity { get; } = new();
    private LineSeries Pressure { get; } = new();

    public bool ShowTemperature { get; set; } = true;
    public bool ShowHumidity { get; set; } = true;
    public bool ShowPressure { get; set; } = true;

    public override async Task BuildAsync(string data)
    {
        if (ShowTemperature)
            await foreach (var point in LineSeriesParser.ParseTemperatureAsync(data))
                Temperature.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));

        if (ShowHumidity)
            await foreach (var point in LineSeriesParser.ParseHumidityAsync(data))
                Humidity.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));

        if (ShowPressure)
            await foreach (var point in LineSeriesParser.ParsePressureAsync(data))
                Pressure.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));

        CreateModel();
    }

    private void CreateModel()
    {
        var model = new PlotModel();

        if (ShowTemperature) model.Series.Add(Temperature);
        if (ShowHumidity) model.Series.Add(Humidity);
        if (ShowPressure) model.Series.Add(Pressure);

        PlotModel = model;
    }
}