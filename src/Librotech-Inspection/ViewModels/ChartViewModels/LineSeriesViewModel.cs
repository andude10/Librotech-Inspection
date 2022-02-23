using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Librotech_Inspection.Utilities.ChartCustomizers;
using Librotech_Inspection.Utilities.Parsers.ChartDataParsers;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReactiveUI;

namespace Librotech_Inspection.ViewModels.ChartViewModels;

/// <summary>
///     LineSeriesViewModel represents the chart, is responsible for plotting the chart
/// </summary>
public sealed class LineSeriesViewModel : ChartViewModel
{
    private PlotModel _plotModel;
    private IChartCustomizer _chartCustomizer;
    
    public override PlotModel PlotModel
    {
        get => _plotModel;
        set
        {
            this.RaiseAndSetIfChanged(ref _plotModel, value);
            UpdatePlotView.Handle(Unit.Default);
        }
    }

    public LineSeriesViewModel(IChartCustomizer chartCustomizer)
    {
        _chartCustomizer = chartCustomizer;
        PlotModel = new PlotModel();
    }

    private LineSeries Temperature { get; set; }
    private LineSeries Humidity { get; set; }
    private LineSeries Pressure { get; set; }
    
    private LinearAxis TemperatureYAxis { get; set; }
    private LinearAxis HumidityYAxis { get; set; }
    private LinearAxis PressureYAxis { get; set; }
    
    private DateTimeAxis XAxis { get; set; }

    public bool ShowTemperature { get; set; } = true;
    public bool ShowHumidity { get; set; } = true;
    public bool ShowPressure { get; set; } = true;

    public override async Task BuildAsync(string data)
    {
        ClearChart();
        
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

        if (ShowTemperature & Temperature.Points.Count != 0)
        {
            model.Axes.Add(TemperatureYAxis);
            model.Series.Add(Temperature);
        }

        if (ShowHumidity & Humidity.Points.Count != 0)
        {
            model.Axes.Add(HumidityYAxis);
            model.Series.Add(Humidity);
        }

        if (ShowPressure & Pressure.Points.Count != 0)
        {
            model.Axes.Add(PressureYAxis);
            model.Series.Add(Pressure);
        }

        var testMax = Temperature.Points.MaxBy(x => x.Y);
        var testMax2 = Humidity.Points.MaxBy(x => x.Y);
        model.Axes.Add(XAxis);

        _chartCustomizer.Customize(model);
        
        PlotModel = model;
    }

    private void ClearChart()
    {
        PlotModel.Series.Clear();
        PlotModel.Axes.Clear();
        
        Temperature = new LineSeries() { Tag = ChartElementTags.LineSeriesTemperature };
        Humidity = new LineSeries() { Tag = ChartElementTags.LineSeriesHumidity };
        Pressure = new LineSeries { Tag = ChartElementTags.LineSeriesPressure };
        
        TemperatureYAxis = new LinearAxis() { Tag = ChartElementTags.TemperatureYAxis };
        HumidityYAxis = new LinearAxis() { Tag = ChartElementTags.HumidityYAxis };
        PressureYAxis = new LinearAxis() { Tag = ChartElementTags.PressureYAxis };

        XAxis = new DateTimeAxis {Tag = ChartElementTags.DateTimeAxis};
    }
}