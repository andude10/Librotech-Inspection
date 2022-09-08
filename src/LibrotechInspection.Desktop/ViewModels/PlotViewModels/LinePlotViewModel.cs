using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Desktop.Models;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.Utilities.Exceptions;
using LibrotechInspection.Desktop.Views.Controls;
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
        Controller.BindMouseDown(OxyMouseButton.Left, new DelegatePlotCommand<OxyMouseDownEventArgs>(
            (view, controller, args) =>
            {
                if (args.HitTestResult is not {Element: LineSeries series, Item: DataPoint dataPoint}) return;

                SelectedPoint = new SelectedDataPoint(dataPoint, series);

                var trackerManipulator = new CustomPlotTrackerManipulator(view);
                controller.AddMouseManipulator(view, trackerManipulator, args);
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

        if (HasTemperature) PlotModelManager.AddTemperature(PlotDataContainer.TemperaturePoints);
        if (HasHumidity) PlotModelManager.AddHumidity(PlotDataContainer.HumidityPoints);
        if (HasPressure) PlotModelManager.AddPressure(PlotDataContainer.PressurePoints);
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

    public void MarkSelectedPoint()
    {
        if (SelectedPoint is null) return;

        PlotModelManager.MarkPoint(SelectedPoint.Point, SelectedPoint.ParentElement);
    }

#endregion
}