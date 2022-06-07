using System;
using System.Reactive;
using System.Threading.Tasks;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Services;
using LibrotechInspection.Desktop.Utilities.Interactions;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReactiveUI;
using Splat;

namespace LibrotechInspection.Desktop.ViewModels.PlotViewModels;

/// <summary>
///     LinePlotViewModel represents the chart, is responsible for plotting the chart
/// </summary>
public sealed class LinePlotViewModel : PlotViewModel
{
    private readonly IPlotCustomizer _plotCustomizer;
    private readonly IPlotDataParser _plotDataParser;
    private PlotModel? _plotModel;

    public LinePlotViewModel(IPlotCustomizer chartCustomizer)
    {
        _plotCustomizer = chartCustomizer;
        _plotModel = new PlotModel();
        _plotDataParser = Locator.Current.GetService<IPlotDataParser>()
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

    private LineSeries Temperature { get; } = new() {Tag = PlotElementTags.SeriesTemperature};
    private LineSeries Humidity { get; } = new() {Tag = PlotElementTags.SeriesHumidity};
    private LineSeries Pressure { get; } = new() {Tag = PlotElementTags.SeriesPressure};

    private LinearAxis TemperatureYAxis { get; } = new() {Tag = PlotElementTags.TemperatureYAxis};
    private LinearAxis HumidityYAxis { get; } = new() {Tag = PlotElementTags.HumidityYAxis};
    private LinearAxis PressureYAxis { get; } = new() {Tag = PlotElementTags.PressureYAxis};

    private DateTimeAxis XAxis { get; } = new() {Tag = PlotElementTags.DateTimeAxis};

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
        {
            await foreach (var point in _plotDataParser.ParseTemperatureAsync(chartData))
                Temperature.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));

            HasTemperature = Temperature.Points.Count > 0;
        }

        if (ShowHumidity)
        {
            await foreach (var point in _plotDataParser.ParseHumidityAsync(chartData))
                Humidity.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));

            HasHumidity = Humidity.Points.Count > 0;
        }

        if (ShowPressure)
        {
            await foreach (var point in _plotDataParser.ParsePressureAsync(chartData))
                Pressure.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));

            HasTemperature = Temperature.Points.Count > 0;
        }

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

        _plotCustomizer.Customize(model);

        PlotModel = model;
    }

#endregion
}