using System;
using System.Threading.Tasks;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Services;
using NLog;
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
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ILinePlotOptimizer _optimizer;
    private readonly IPlotCustomizer _plotCustomizer;
    private readonly IPlotDataParser _plotDataParser;
    private PlotModel _plotModel;

    public LinePlotViewModel(IPlotCustomizer chartCustomizer)
    {
        _plotCustomizer = chartCustomizer;
        _plotModel = new PlotModel();
        _plotDataParser = Locator.Current.GetService<IPlotDataParser>()
                          ?? throw new InvalidOperationException();
        _optimizer = Locator.Current.GetService<ILinePlotOptimizer>()
                     ?? throw new InvalidOperationException();

        Initialize();
    }

    public LinePlotViewModel(IPlotCustomizer chartCustomizer, IPlotDataParser plotDataParser,
        ILinePlotOptimizer optimizer)
    {
        _plotCustomizer = chartCustomizer;
        _plotModel = new PlotModel();
        _plotDataParser = plotDataParser;
        _optimizer = optimizer;

        Initialize();
    }

    public override PlotModel PlotModel
    {
        get => _plotModel;
        set => this.RaiseAndSetIfChanged(ref _plotModel, value);
    }

    public override IObservable<PlotModel> PlotModelUpdate { get; protected set; }

    private void Initialize()
    {
        this.WhenAnyValue(vm => vm.ShowTemperature,
                vm => vm.ShowHumidity,
                vm => vm.ShowPressure)
            .Subscribe(_ => CreateModel());

        PlotModelUpdate = this.WhenAnyValue(vm => vm.PlotModel);
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
        Logger.Info("Start building a chart with the following parameters:" +
                    $"ShowTemperature: {ShowTemperature}, ShowHumidity: {ShowHumidity}, ShowPressure: {ShowPressure}.");

        Temperature.Points.Clear();
        Humidity.Points.Clear();
        Pressure.Points.Clear();

        if (ShowTemperature)
        {
            Logger.Debug("Start parse Temperature series...");
            try
            {
                await foreach (var point in _plotDataParser.ParseTemperatureAsync(chartData))
                    Temperature.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));
                Logger.Debug(
                    $"Temperature series parsing complited, total number of points is '{Temperature.Points.Count}'.");
            }
            catch (Exception e)
            {
                Logger.Error(e, "An unexpected error occurred while parsing Temperature series.");
                throw;
            }

            HasTemperature = Temperature.Points.Count > 0;

            if (HasTemperature)
            {
                Logger.Debug("Start Temperature series optimization.");
                await _optimizer.OptimizeAsync(Temperature.Points);
                Logger.Debug(
                    $"Temperature series optimization complited, result number of points is '{Temperature.Points.Count}'.");
            }
        }

        if (ShowHumidity)
        {
            Logger.Debug("Start parse Humidity series...");
            try
            {
                await foreach (var point in _plotDataParser.ParseHumidityAsync(chartData))
                    Humidity.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));
                Logger.Debug(
                    $"Humidity series parsing complited, total number of points is '{Humidity.Points.Count}'.");
            }
            catch (Exception e)
            {
                Logger.Error(e, "An unexpected error occurred while parsing Humidity series.");
                throw;
            }

            HasHumidity = Humidity.Points.Count > 0;

            if (HasHumidity)
            {
                Logger.Debug("Start Humidity series optimization.");
                await _optimizer.OptimizeAsync(Humidity.Points);
                Logger.Debug(
                    $"Humidity series optimization complited, result number of points is '{Humidity.Points.Count}'.");
            }
        }

        if (ShowPressure)
        {
            Logger.Debug("Start parse Pressure series...");
            try
            {
                await foreach (var point in _plotDataParser.ParsePressureAsync(chartData))
                    Pressure.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));
                Logger.Debug(
                    $"Pressure series parsing complited, total number of points is '{Pressure.Points.Count}'.");
            }
            catch (Exception e)
            {
                Logger.Error(e, "An unexpected error occurred while parsing Pressure series.");
                throw;
            }

            HasPressure = Pressure.Points.Count > 0;

            if (HasHumidity)
            {
                Logger.Debug("Start Pressure series optimization.");
                await _optimizer.OptimizeAsync(Pressure.Points);
                Logger.Debug(
                    $"Pressure series optimization complited, result number of points is '{Pressure.Points.Count}'.");
            }
        }

        CreateModel();
    }

    public override void CreateModel()
    {
        Logger.Info("Start creating and configure a chart model.");

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