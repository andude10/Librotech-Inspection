using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Services;
using LibrotechInspection.Desktop.Utilities.Exceptions;
using LibrotechInspection.Desktop.Utilities.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Splat;

namespace LibrotechInspection.Desktop.Services;

public class LinePlotModelManager
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

    [JsonConstructor]
    public LinePlotModelManager(IEnumerable<SerializableDataPoint>? temperaturePoints,
        IEnumerable<SerializableDataPoint>? humidityPoints,
        IEnumerable<SerializableDataPoint>? pressurePoints,
        IEnumerable<SerializableDataPoint>? temperatureMarkedPoints,
        IEnumerable<SerializableDataPoint>? humidityMarkedPoints,
        IEnumerable<SerializableDataPoint>? pressureMarkedPoints) : this()
    {
        if (temperaturePoints is not null)
            TemperatureSeries.Points.AddRange(temperaturePoints.Select(p => p.GetDataPoint()));
        if (humidityPoints is not null)
            HumiditySeries.Points.AddRange(humidityPoints.Select(p => p.GetDataPoint()));
        if (pressurePoints is not null)
            PressureSeries.Points.AddRange(pressurePoints.Select(p => p.GetDataPoint()));

        if (temperatureMarkedPoints is not null)
            TemperatureMarkedSeries.Points.AddRange(temperatureMarkedPoints.Select(p => p.GetDataPoint()));
        if (humidityMarkedPoints is not null)
            HumidityMarkedSeries.Points.AddRange(humidityMarkedPoints.Select(p => p.GetDataPoint()));
        if (pressureMarkedPoints is not null)
            PressureMarkedSeries.Points.AddRange(pressureMarkedPoints.Select(p => p.GetDataPoint()));

        AutoBuildModel();
    }

#region Properties

    [JsonIgnore] public LineSeries TemperatureSeries { get; }
    [JsonIgnore] public LineSeries HumiditySeries { get; }
    [JsonIgnore] public LineSeries PressureSeries { get; }

    [JsonIgnore] public LinearAxis TemperatureYAxis { get; }
    [JsonIgnore] public LinearAxis HumidityYAxis { get; }
    [JsonIgnore] public LinearAxis PressureYAxis { get; }

    [JsonIgnore] public DateTimeAxis DateTimeAxis { get; }

    [JsonIgnore] public LineSeries TemperatureMarkedSeries { get; }
    [JsonIgnore] public LineSeries HumidityMarkedSeries { get; }
    [JsonIgnore] public LineSeries PressureMarkedSeries { get; }

    [JsonIgnore] public PlotModel PlotModel { get; }

#endregion

#region Methods

    public void AddSeparatorLine(DataPoint point)
    {
    }

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

    public void AutoBuildModel()
    {
        AddDateTimeAxis();

        if (TemperatureSeries.Points.Count != 0) AddTemperature();
        if (HumiditySeries.Points.Count != 0) AddHumidity();
        if (PressureSeries.Points.Count != 0) AddPressure();

        if (TemperatureMarkedSeries.Points.Count != 0) AddTemperatureMarkedSeries();
        if (HumidityMarkedSeries.Points.Count != 0) AddHumidityMarkedSeries();
        if (PressureMarkedSeries.Points.Count != 0) AddPressureMarkedSeries();

        UpdatePlotView();
    }

    public void AddDateTimeAxis()
    {
        if (PlotModel.Axes.Contains(DateTimeAxis)) return;

        PlotModel.Axes.Add(DateTimeAxis);

        UpdatePlotView();
    }

    public void AddTemperature()
    {
        if (!PlotModel.Series.Contains(TemperatureSeries)) InsertTemperatureSeriesAndAxis();

        UpdatePlotView();
    }

    public void AddTemperatureMarkedSeries()
    {
        if (!PlotModel.Series.Contains(TemperatureMarkedSeries)) PlotModel.Series.Add(TemperatureMarkedSeries);

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

    public void AddHumidity()
    {
        if (!PlotModel.Series.Contains(HumiditySeries)) InsertHumiditySeriesAndAxis();

        UpdatePlotView();
    }

    public void AddHumidityMarkedSeries()
    {
        if (!PlotModel.Series.Contains(HumidityMarkedSeries)) PlotModel.Series.Add(HumidityMarkedSeries);

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

    public void AddPressure()
    {
        if (!PlotModel.Series.Contains(PressureSeries)) InsertPressureSeriesAndAxis();

        UpdatePlotView();
    }

    public void AddPressureMarkedSeries()
    {
        if (!PlotModel.Series.Contains(PressureMarkedSeries)) PlotModel.Series.Add(PressureMarkedSeries);

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

#region Data members

    [JsonInclude]
    public IEnumerable<SerializableDataPoint> TemperaturePoints =>
        TemperatureSeries.Points.Select(p => new SerializableDataPoint(p.X, p.Y));

    [JsonInclude]
    public IEnumerable<SerializableDataPoint> HumidityPoints =>
        HumiditySeries.Points.Select(p => new SerializableDataPoint(p.X, p.Y));

    [JsonInclude]
    public IEnumerable<SerializableDataPoint> PressurePoints =>
        PressureSeries.Points.Select(p => new SerializableDataPoint(p.X, p.Y));


    [JsonInclude]
    public IEnumerable<SerializableDataPoint> TemperatureMarkedPoints =>
        TemperatureMarkedSeries.Points.Select(p => new SerializableDataPoint(p.X, p.Y));

    [JsonInclude]
    public IEnumerable<SerializableDataPoint> HumidityMarkedPoints =>
        HumidityMarkedSeries.Points.Select(p => new SerializableDataPoint(p.X, p.Y));

    [JsonInclude]
    public IEnumerable<SerializableDataPoint> PressureMarkedPoints =>
        PressureMarkedSeries.Points.Select(p => new SerializableDataPoint(p.X, p.Y));

#endregion
}