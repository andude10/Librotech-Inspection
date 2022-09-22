using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Desktop.Models;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.Utilities.Enums;
using LibrotechInspection.Desktop.Utilities.Exceptions;
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
public sealed class LinePlotViewModel : LinePlotViewModelBase
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ILinePlotOptimizer _optimizer;
    private readonly IPlotDataParser _plotDataParser;

    public LinePlotViewModel(string textDataForPlot) : this()
    {
        TextDataForPlot = textDataForPlot;
    }

    public LinePlotViewModel()
    {
        ModelManager = new LinePlotModelManager();
        Controller = new PlotController();

        _plotDataParser = Locator.Current.GetService<IPlotDataParser>()
                          ?? throw new NoServiceFound(nameof(IPlotDataParser));
        _optimizer = Locator.Current.GetService<ILinePlotOptimizer>()
                     ?? throw new NoServiceFound(nameof(ILinePlotOptimizer));

        MarkSelectedPointCommand = ReactiveCommand.Create(MarkSelectedPoint);
        CreateSeparatorLineCommand = ReactiveCommand.Create(CreateSeparatorLine);
        ZoomInCommand = ReactiveCommand.Create(() => Zoom(1));
        ZoomOutCommand = ReactiveCommand.Create(() => Zoom(-1));

        InitializePlotController();

        SelectedToolObservable.Subscribe(plotTool =>
        {
            switch (plotTool)
            {
                case PlotTool.SelectionZoom:
                    SetupSelectionZoomController();
                    break;
                case PlotTool.Panning:
                    SetupPanningController();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(plotTool), plotTool, null);
            }
        });

        this.WhenAnyValue(vm => vm.DisplayConditions.DisplayTemperature)
            .Select(display => display && HasTemperature)
            .Subscribe(display =>
            {
                ModelManager.ShowOrHideTemperature(display);
                ModelManager.UpdatePlotView();
            });
        this.WhenAnyValue(vm => vm.DisplayConditions.DisplayHumidity)
            .Select(display => display && HasHumidity)
            .Subscribe(display =>
            {
                ModelManager.ShowOrHideHumidity(display);
                ModelManager.UpdatePlotView();
            });
        this.WhenAnyValue(vm => vm.DisplayConditions.DisplayPressure)
            .Select(display => display && HasPressure)
            .Subscribe(display =>
            {
                ModelManager.ShowOrHidePressure(display);
                ModelManager.UpdatePlotView();
            });
    }

    private void InitializePlotController()
    {
        Controller.BindMouseDown(OxyMouseButton.Right, new DelegatePlotCommand<OxyMouseDownEventArgs>(
            (view, controller, args) =>
            {
                SelectedPoint = args.HitTestResult is {Element: LineSeries series, Item: DataPoint dataPoint}
                    ? new SelectedDataPoint(dataPoint, series)
                    : null;
            }));
        Controller.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Alt, PlotCommands.Track);
    }

    private void SetupSelectionZoomController()
    {
        Controller.BindMouseDown(OxyMouseButton.Left, PlotCommands.ZoomRectangle);
    }

    private void SetupPanningController()
    {
        Controller.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);
    }

    private void Zoom(double delta)
    {
        ModelManager.PlotModel.ZoomAllAxes(1 + delta * 0.12);
        ModelManager.PlotModel.PlotView?.InvalidatePlot();
    }

#region Commands

    [JsonIgnore] public override ReactiveCommand<Unit, Unit> MarkSelectedPointCommand { get; }
    [JsonIgnore] public override ReactiveCommand<Unit, Unit> CreateSeparatorLineCommand { get; }
    [JsonIgnore] public override ReactiveCommand<Unit, Unit> ZoomInCommand { get; }
    [JsonIgnore] public override ReactiveCommand<Unit, Unit> ZoomOutCommand { get; }

#endregion

#region Properties

    [JsonInclude] public override bool HasTemperature => ModelManager.TemperaturePoints.Any();
    [JsonInclude] public override bool HasHumidity => ModelManager.HumidityPoints.Any();
    [JsonInclude] public override bool HasPressure => ModelManager.PressurePoints.Any();
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

        ModelManager.UpdatePlotView();
    }

    private async Task ParseTextDataToLinePlotData()
    {
        if (TextDataForPlot is null) throw new InvalidOperationException("Unexpected ChartData null value.");

        try
        {
            Logger.Info("Parsing temperature series...");
            await foreach (var point in _plotDataParser.ParseTemperatureAsync(TextDataForPlot))
                ModelManager.TemperatureSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));
            Logger.Info("Completed");

            Logger.Info("Parsing humidity series...");
            await foreach (var point in _plotDataParser.ParseHumidityAsync(TextDataForPlot))
                ModelManager.HumiditySeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));
            Logger.Info("Completed");

            Logger.Info("Parsing pressure series...");
            await foreach (var point in _plotDataParser.ParsePressureAsync(TextDataForPlot))
                ModelManager.PressureSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.X), point.Y));
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
            await _optimizer.OptimizeAsync(ModelManager.TemperatureSeries.Points);
            Logger.Info("Completed");
        }

        if (HasHumidity)
        {
            Logger.Info("Optimizing humidity series optimization.");
            await _optimizer.OptimizeAsync(ModelManager.HumiditySeries.Points);
            Logger.Info("Completed");
        }

        if (HasPressure)
        {
            Logger.Info("Optimizing pressure series optimization.");
            await _optimizer.OptimizeAsync(ModelManager.PressureSeries.Points);
            Logger.Info("Completed");
        }
    }

    private void MarkSelectedPoint()
    {
        if (SelectedPoint?.ParentElement is not Series series) return;

        ModelManager.MarkPoint(SelectedPoint.Point, series);
        ModelManager.UpdatePlotView();
    }

    private void CreateSeparatorLine()
    {
        if (SelectedPoint?.ParentElement is not Series) return;

        ModelManager.CreateSeparatorLine(SelectedPoint.Point.X);
        ModelManager.UpdatePlotView();
    }

#endregion
}