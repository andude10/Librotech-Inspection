using System.Collections.Generic;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Services;
using LibrotechInspection.Desktop.Utilities.Exceptions;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Splat;

namespace LibrotechInspection.Desktop.Services;

public class LinePlotModelManager : IPlotModelManager
{
    private readonly IPlotCustomizer _plotCustomizer;

    public LinePlotModelManager()
    {
        _plotCustomizer ??= Locator.Current.GetService<IPlotCustomizer>()
                            ?? throw new NoServiceFound(nameof(IPlotCustomizer));
        PlotModel = new PlotModel();
        TemperatureSeries = new LineSeries {Tag = PlotElementTags.SeriesTemperature};
        HumiditySeries = new LineSeries {Tag = PlotElementTags.SeriesHumidity};
        PressureSeries = new LineSeries {Tag = PlotElementTags.SeriesPressure};

        TemperatureYAxis = new LinearAxis {Tag = PlotElementTags.TemperatureYAxis};
        HumidityYAxis = new LinearAxis {Tag = PlotElementTags.HumidityYAxis};
        PressureYAxis = new LinearAxis {Tag = PlotElementTags.PressureYAxis};

        DateTimeAxis = new DateTimeAxis {Tag = PlotElementTags.DateTimeAxis};

        TemperatureMarkedSeries = new LineSeries {Tag = PlotElementTags.SeriesTemperatureMarked};
        HumidityMarkedSeries = new LineSeries {Tag = PlotElementTags.SeriesHumidityMarked};
        PressureMarkedSeries = new LineSeries {Tag = PlotElementTags.SeriesPressureMarked};
    }

#region Properties

    private LineSeries TemperatureSeries { get; }
    private LineSeries HumiditySeries { get; }
    private LineSeries PressureSeries { get; }

    private LinearAxis TemperatureYAxis { get; }
    private LinearAxis HumidityYAxis { get; }
    private LinearAxis PressureYAxis { get; }

    private DateTimeAxis DateTimeAxis { get; }

    private LineSeries TemperatureMarkedSeries { get; }
    private LineSeries HumidityMarkedSeries { get; }
    private LineSeries PressureMarkedSeries { get; }

    public PlotModel PlotModel { get; }

#endregion

#region Methods

    public void MarkPoint(DataPoint point, Element parentElement)
    {
        if (parentElement == TemperatureSeries)
            TemperatureMarkedSeries.Points.Add(point);
        else if (parentElement == HumiditySeries)
            HumidityMarkedSeries.Points.Add(point);
        else if (parentElement == TemperatureSeries) PressureMarkedSeries.Points.Add(point);

        UpdatePlotView();
    }

    public void AddDateTimeAxis()
    {
        if (PlotModel.Axes.Contains(DateTimeAxis)) return;

        PlotModel.Axes.Add(DateTimeAxis);

        UpdatePlotView();
    }

    public void AddTemperature(IEnumerable<DataPoint> temperaturePoints)
    {
        if (!PlotModel.Series.Contains(TemperatureSeries))
        {
            PlotModel.Series.Add(TemperatureSeries);
            PlotModel.Axes.Add(TemperatureYAxis);
        }

        TemperatureSeries.Points.Clear();
        TemperatureSeries.Points.AddRange(temperaturePoints);

        UpdatePlotView();
    }

    public void ShowOrHideTemperature(bool display)
    {
        if (display)
        {
            if (PlotModel.Series.Contains(TemperatureSeries)) return;

            PlotModel.Series.Add(TemperatureSeries);
            PlotModel.Axes.Add(TemperatureYAxis);
        }
        else
        {
            if (!PlotModel.Series.Contains(TemperatureSeries)) return;

            PlotModel.Series.Remove(TemperatureSeries);
            PlotModel.Axes.Remove(TemperatureYAxis);
        }

        UpdatePlotView();
    }

    public void AddHumidity(IEnumerable<DataPoint> humidityPoints)
    {
        if (!PlotModel.Series.Contains(HumiditySeries))
        {
            PlotModel.Series.Add(HumiditySeries);
            PlotModel.Axes.Add(HumidityYAxis);
        }

        HumiditySeries.Points.Clear();
        HumiditySeries.Points.AddRange(humidityPoints);

        UpdatePlotView();
    }

    public void ShowOrHideHumidity(bool display)
    {
        if (display)
        {
            if (PlotModel.Series.Contains(HumiditySeries)) return;

            PlotModel.Series.Add(HumiditySeries);
            PlotModel.Axes.Add(HumidityYAxis);
        }
        else
        {
            if (!PlotModel.Series.Contains(HumiditySeries)) return;

            PlotModel.Series.Remove(HumiditySeries);
            PlotModel.Axes.Remove(HumidityYAxis);
        }

        UpdatePlotView();
    }

    public void AddPressure(IEnumerable<DataPoint> pressurePoints)
    {
        if (!PlotModel.Series.Contains(PressureSeries))
        {
            PlotModel.Series.Add(PressureSeries);
            PlotModel.Axes.Add(PressureYAxis);
        }

        PressureSeries.Points.Clear();
        PressureSeries.Points.AddRange(pressurePoints);

        UpdatePlotView();
    }

    public void ShowOrHidePressure(bool display)
    {
        if (display)
        {
            if (PlotModel.Series.Contains(PressureSeries)) return;

            PlotModel.Series.Add(PressureSeries);
            PlotModel.Axes.Add(PressureYAxis);
        }
        else
        {
            if (!PlotModel.Series.Contains(PressureSeries)) return;

            PlotModel.Series.Remove(PressureSeries);
            PlotModel.Axes.Remove(PressureYAxis);
        }

        UpdatePlotView();
    }

    private void UpdatePlotView()
    {
        PlotModel.PlotView?.InvalidatePlot();
        _plotCustomizer.Customize(PlotModel);
    }

#endregion
}