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

    public void MarkPoint(DataPoint point, Series parentSeries)
    {
        if (parentSeries == TemperatureSeries)
        {
            if (!PlotModel.Series.Contains(TemperatureMarkedSeries))
                PlotModel.Series.Add(TemperatureMarkedSeries);

            TemperatureMarkedSeries.Points.Add(point);
        }
        else if (parentSeries == HumiditySeries)
        {
            if (!PlotModel.Series.Contains(HumidityMarkedSeries))
                PlotModel.Series.Add(HumidityMarkedSeries);

            HumidityMarkedSeries.Points.Add(point);
        }
        else if (parentSeries == TemperatureSeries)
        {
            if (!PlotModel.Series.Contains(PressureMarkedSeries))
                PlotModel.Series.Add(PressureMarkedSeries);

            PressureMarkedSeries.Points.Add(point);
        }

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
        if (!PlotModel.Series.Contains(TemperatureSeries)) InsertTemperatureSeriesAndAxis();

        TemperatureSeries.Points.Clear();
        TemperatureSeries.Points.AddRange(temperaturePoints);

        UpdatePlotView();
    }

    public void AddTemperatureMarkedPoints(IEnumerable<DataPoint> temperaturePoints)
    {
        if (!PlotModel.Series.Contains(TemperatureMarkedSeries)) PlotModel.Series.Add(TemperatureMarkedSeries);

        TemperatureMarkedSeries.Points.Clear();
        TemperatureMarkedSeries.Points.AddRange(temperaturePoints);

        UpdatePlotView();
    }

    public void ShowOrHideTemperature(bool display)
    {
        if (display)
        {
            if (PlotModel.Series.Contains(TemperatureSeries)) return;

            InsertTemperatureSeriesAndAxis();
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
        if (!PlotModel.Series.Contains(HumiditySeries)) InsertHumiditySeriesAndAxis();

        HumiditySeries.Points.Clear();
        HumiditySeries.Points.AddRange(humidityPoints);

        UpdatePlotView();
    }

    public void AddHumidityMarkedPoints(IEnumerable<DataPoint> humidityPoints)
    {
        if (!PlotModel.Series.Contains(HumidityMarkedSeries)) PlotModel.Series.Add(HumidityMarkedSeries);

        HumidityMarkedSeries.Points.Clear();
        HumidityMarkedSeries.Points.AddRange(humidityPoints);

        UpdatePlotView();
    }

    public void ShowOrHideHumidity(bool display)
    {
        if (display)
        {
            if (PlotModel.Series.Contains(HumiditySeries)) return;

            InsertHumiditySeriesAndAxis();
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
        if (!PlotModel.Series.Contains(PressureSeries)) InsertPressureSeriesAndAxis();

        PressureSeries.Points.Clear();
        PressureSeries.Points.AddRange(pressurePoints);

        UpdatePlotView();
    }

    public void AddPressureMarkedPoints(IEnumerable<DataPoint> pressurePoints)
    {
        if (!PlotModel.Series.Contains(PressureMarkedSeries)) PlotModel.Series.Add(PressureMarkedSeries);

        PressureMarkedSeries.Points.Clear();
        PressureMarkedSeries.Points.AddRange(pressurePoints);

        UpdatePlotView();
    }

    public void ShowOrHidePressure(bool display)
    {
        if (display)
        {
            if (PlotModel.Series.Contains(PressureSeries)) return;

            InsertPressureSeriesAndAxis();
        }
        else
        {
            if (!PlotModel.Series.Contains(PressureSeries)) return;

            PlotModel.Series.Remove(PressureSeries);
            PlotModel.Axes.Remove(PressureYAxis);
        }

        UpdatePlotView();
    }

#region Helper Methods

    private void UpdatePlotView()
    {
        _plotCustomizer.Customize(PlotModel);
        PlotModel.PlotView?.InvalidatePlot();
    }

    private void InsertTemperatureSeriesAndAxis()
    {
        PlotModel.Series.Insert(0, TemperatureSeries);
        PlotModel.Axes.Insert(0, TemperatureYAxis);
    }

    private void InsertHumiditySeriesAndAxis()
    {
        PlotModel.Series.Insert(1, HumiditySeries);
        PlotModel.Axes.Insert(1, HumidityYAxis);
    }

    private void InsertPressureSeriesAndAxis()
    {
        if (PlotModel.Series.Contains(PressureSeries)) return;

        PlotModel.Series.Insert(2, PressureSeries);
        PlotModel.Axes.Insert(2, PressureYAxis);
    }

#endregion

#endregion
}