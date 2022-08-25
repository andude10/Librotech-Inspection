using System;
using System.Reactive;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.Utilities.Exceptions;
using NLog;
using OxyPlot;
using OxyPlot.Axes;
using ReactiveUI;
using Splat;

namespace LibrotechInspection.Desktop.ViewModels.PlotViewModels;

/// <summary>
///     LinePlotViewModel represents the chart, is responsible for plotting the chart
/// </summary>
public sealed class LinePlotViewModel : PlotViewModel
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly IPlotElementProvider _elementProvider;
    private readonly ILinePlotOptimizer _optimizer;
    private readonly IPlotCustomizer _plotCustomizer;
    private readonly IPlotDataParser _plotDataParser;

    [JsonConstructor]
    public LinePlotViewModel(LinePlotData plotData)
    {
        PlotData = plotData;
    }

    public LinePlotViewModel(string? textDataForPlot = null,
        IPlotCustomizer? chartCustomizer = null,
        IPlotDataParser? plotDataParser = null,
        ILinePlotOptimizer? optimizer = null,
        IPlotElementProvider? elementProvider = null)
    {
        TextDataForPlot = textDataForPlot;
        PlotData = new LinePlotData();
        _plotCustomizer = chartCustomizer ?? Locator.Current.GetService<IPlotCustomizer>()
            ?? throw new NoServiceFound(nameof(IPlotCustomizer));
        _plotDataParser = plotDataParser ?? Locator.Current.GetService<IPlotDataParser>()
            ?? throw new NoServiceFound(nameof(IPlotDataParser));
        _optimizer = optimizer ?? Locator.Current.GetService<ILinePlotOptimizer>()
            ?? throw new NoServiceFound(nameof(ILinePlotOptimizer));
        _elementProvider = elementProvider ?? Locator.Current.GetService<IPlotElementProvider>()
            ?? throw new NoServiceFound(nameof(IPlotElementProvider));

        UpdateModel = ReactiveCommand.Create(CreateModel);
        DisplayConditionsChange.InvokeCommand(UpdateModel);
    }

#region Commands

    private ReactiveCommand<Unit, Unit> UpdateModel { get; }

#endregion

#region Properties

    [JsonInclude] public LinePlotData PlotData { get; set; }

    [JsonInclude] public override string PlotType { get; } = nameof(LinePlotViewModel);

#endregion

#region Methods

    public override async Task BuildAsync()
    {
        if (TextDataForPlot is null)
        {
            Logger.Error("No chart data provided to LinePlotViewModel");
            return;
        }

        await ParseLinePlotData();
        await OptimizeDataPoints();

        CreateModel();
    }

    private void CreateModel()
    {
        Logger.Info("Creating and configure a plot model...");

        PlotModel.Axes.Clear();
        PlotModel.Series.Clear();

        var model = new PlotModel();

        // TODO: improve display condition validation (remove if's)
        if (HasTemperature & DisplayConditions.DisplayTemperature)
        {
            var temperatureSeries = _elementProvider.GetTemperatureSeries();
            temperatureSeries.Points.AddRange(PlotData.TemperaturePoints);

            model.Series.Add(temperatureSeries);
            model.Axes.Add(_elementProvider.GetTemperatureYAxis());
        }

        if (HasHumidity & DisplayConditions.DisplayHumidity)
        {
            var humiditySeries = _elementProvider.GetHumiditySeries();
            humiditySeries.Points.AddRange(PlotData.TemperaturePoints);

            model.Series.Add(humiditySeries);
            model.Axes.Add(_elementProvider.GetHumidityYAxis());
        }

        if (HasPressure & DisplayConditions.DisplayPressure)
        {
            var pressureSeries = _elementProvider.GetHumiditySeries();
            pressureSeries.Points.AddRange(PlotData.TemperaturePoints);

            model.Series.Add(pressureSeries);
            model.Axes.Add(_elementProvider.GetPressureYAxis());
        }

        model.Axes.Add(_elementProvider.GetXAxis());

        _plotCustomizer.Customize(model);

        PlotModel = model;
        Logger.Info("Completed");
    }

    private async Task ParseLinePlotData()
    {
        if (TextDataForPlot is null) throw new InvalidOperationException("Unexpected ChartData null value.");

        try
        {
            Logger.Info("Parsing temperature series...");
            await foreach (var point in _plotDataParser.ParseTemperatureAsync(TextDataForPlot))
                PlotData.TemperaturePoints.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));
            Logger.Info("Completed");

            Logger.Info("Parsing humidity series...");
            await foreach (var point in _plotDataParser.ParseHumidityAsync(TextDataForPlot))
                PlotData.HumidityPoints.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));
            Logger.Info("Completed");

            Logger.Info("Parsing pressure series...");
            await foreach (var point in _plotDataParser.ParsePressureAsync(TextDataForPlot))
                PlotData.PressurePoints.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));
            Logger.Info("Completed");

            HasTemperature = PlotData.TemperaturePoints.Count != 0;
            HasHumidity = PlotData.HumidityPoints.Count != 0;
            HasPressure = PlotData.PressurePoints.Count != 0;
        }
        catch (Exception e)
        {
            Logger.Error(e, "An unexpected error occurred while parsing LinePlotData");
            throw;
        }
    }

    private async Task OptimizeDataPoints()
    {
        if (HasTemperature)
        {
            Logger.Info("Optimizing temperature series optimization.");
            await _optimizer.OptimizeAsync(PlotData.TemperaturePoints);
            Logger.Info("Completed");
        }

        if (HasHumidity)
        {
            Logger.Info("Optimizing humidity series optimization.");
            await _optimizer.OptimizeAsync(PlotData.HumidityPoints);
            Logger.Info("Completed");
        }

        if (HasPressure)
        {
            Logger.Info("Optimizing pressure series optimization.");
            await _optimizer.OptimizeAsync(PlotData.PressurePoints);
            Logger.Info("Completed");
        }
    }

#endregion
}