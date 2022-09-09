using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Services;
using LibrotechInspection.Desktop.Models;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.Utilities.Exceptions;
using NLog;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace LibrotechInspection.Desktop.ViewModels.PlotViewModels;

/// <summary>
///     LinePlotViewModel represents the chart, is responsible for plotting the chart
/// </summary>
public sealed class LinePlotViewModel : PlotViewModel
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ILinePlotOptimizer _optimizer;
    private readonly IPlotDataParser _plotDataParser;

    [JsonConstructor]
    public LinePlotViewModel(PlotDataContainer plotDataContainer) : this()
    {
        PlotDataContainer = plotDataContainer;
    }

    public LinePlotViewModel(string textDataForPlot) : this()
    {
        TextDataForPlot = textDataForPlot;
    }

    public LinePlotViewModel()
    {
        PlotDataContainer ??= new PlotDataContainer();
        PlotModelManager = new LinePlotModelManager();
        Controller = new PlotController();

        _plotDataParser = Locator.Current.GetService<IPlotDataParser>()
                          ?? throw new NoServiceFound(nameof(IPlotDataParser));
        _optimizer = Locator.Current.GetService<ILinePlotOptimizer>()
                     ?? throw new NoServiceFound(nameof(ILinePlotOptimizer));

        MarkSelectedDataPointCommand = ReactiveCommand.Create(MarkSelectedPoint);

        SetupPlotController();

        this.WhenAnyValue(vm => vm.DisplayConditions.DisplayTemperature)
            .Select(display => display && HasTemperature)
            .Subscribe(display => PlotModelManager.ShowOrHideTemperature(display));
        this.WhenAnyValue(vm => vm.DisplayConditions.DisplayHumidity)
            .Select(display => display && HasHumidity)
            .Subscribe(display => PlotModelManager.ShowOrHideHumidity(display));
        this.WhenAnyValue(vm => vm.DisplayConditions.DisplayPressure)
            .Select(display => display && HasPressure)
            .Subscribe(display => PlotModelManager.ShowOrHidePressure(display));

        this.WhenAnyValue(vm => vm.PlotDataContainer)
            .Subscribe(_ => ConfigurePlotModel());
    }

#region Commands

    [JsonIgnore] public override ReactiveCommand<Unit, Unit> MarkSelectedDataPointCommand { get; }

#endregion

    private void SetupPlotController()
    {
        Controller.BindMouseDown(OxyMouseButton.Right, new DelegatePlotCommand<OxyMouseDownEventArgs>(
            (view, controller, args) =>
            {
                SelectedPoint = args.HitTestResult is {Element: LineSeries series, Item: DataPoint dataPoint}
                    ? new SelectedDataPoint(dataPoint, series)
                    : null;
            }));
    }

#region Properties

    [JsonInclude] public override bool HasTemperature => PlotDataContainer.TemperaturePoints.Any();
    [JsonInclude] public override bool HasHumidity => PlotDataContainer.HumidityPoints.Any();
    [JsonInclude] public override bool HasPressure => PlotDataContainer.PressurePoints.Any();

    [JsonInclude] [Reactive] public PlotDataContainer PlotDataContainer { get; set; }

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

        await ParseTextDataToLinePlotData();
        await OptimizeDataPoints();

        ConfigurePlotModel();
    }

    private void ConfigurePlotModel()
    {
        PlotModelManager.AddDateTimeAxis();

        if (HasTemperature)
        {
            PlotModelManager.AddTemperature(PlotDataContainer.TemperaturePoints);
            PlotModelManager.AddTemperatureMarkedPoints(
                PlotDataContainer.MarkedTemperaturePoints.Select(point => new DataPoint(point.X, point.Y)));
        }

        if (HasHumidity)
        {
            PlotModelManager.AddHumidity(PlotDataContainer.HumidityPoints);
            PlotModelManager.AddHumidityMarkedPoints(
                PlotDataContainer.MarkedHumidityPoints.Select(point => new DataPoint(point.X, point.Y)));
        }

        if (HasPressure)
        {
            PlotModelManager.AddPressure(PlotDataContainer.PressurePoints);
            PlotModelManager.AddPressureMarkedPoints(
                PlotDataContainer.MarkedPressurePoints.Select(point => new DataPoint(point.X, point.Y)));
        }
    }

    private async Task ParseTextDataToLinePlotData()
    {
        if (TextDataForPlot is null) throw new InvalidOperationException("Unexpected ChartData null value.");

        try
        {
            Logger.Info("Parsing temperature series...");
            await foreach (var point in _plotDataParser.ParseTemperatureAsync(TextDataForPlot))
                PlotDataContainer.TemperaturePoints.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));
            Logger.Info("Completed");

            Logger.Info("Parsing humidity series...");
            await foreach (var point in _plotDataParser.ParseHumidityAsync(TextDataForPlot))
                PlotDataContainer.HumidityPoints.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));
            Logger.Info("Completed");

            Logger.Info("Parsing pressure series...");
            await foreach (var point in _plotDataParser.ParsePressureAsync(TextDataForPlot))
                PlotDataContainer.PressurePoints.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));
            Logger.Info("Completed");
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
            await _optimizer.OptimizeAsync(PlotDataContainer.TemperaturePoints);
            Logger.Info("Completed");
        }

        if (HasHumidity)
        {
            Logger.Info("Optimizing humidity series optimization.");
            await _optimizer.OptimizeAsync(PlotDataContainer.HumidityPoints);
            Logger.Info("Completed");
        }

        if (HasPressure)
        {
            Logger.Info("Optimizing pressure series optimization.");
            await _optimizer.OptimizeAsync(PlotDataContainer.PressurePoints);
            Logger.Info("Completed");
        }
    }

    private void MarkSelectedPoint()
    {
        if (SelectedPoint?.ParentElement is not Series series) return;

        if (series.Tag == PlotElementTags.SeriesTemperature)
            PlotDataContainer.MarkedTemperaturePoints.Add(new MarkedDataPoint(SelectedPoint.Point.X,
                SelectedPoint.Point.Y));
        else if (series.Tag == PlotElementTags.SeriesHumidity)
            PlotDataContainer.MarkedHumidityPoints.Add(new MarkedDataPoint(SelectedPoint.Point.X,
                SelectedPoint.Point.Y));
        else if (series.Tag == PlotElementTags.SeriesPressure)
            PlotDataContainer.MarkedPressurePoints.Add(new MarkedDataPoint(SelectedPoint.Point.X,
                SelectedPoint.Point.Y));

        PlotModelManager.MarkPoint(SelectedPoint.Point, series);
    }

#endregion
}