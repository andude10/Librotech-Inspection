using System;
using System.Collections.Generic;
using System.Linq;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Services;
using LibrotechInspection.Desktop.Utilities.Exceptions;
using NLog;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Splat;

namespace LibrotechInspection.Desktop.Models;

public class LinePlotModelBuilder
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private PlotModel _plotModel;

    public LinePlotModelBuilder()
    {
        _plotModel = new PlotModel();
        Logger.Info($"{nameof(LinePlotModelBuilder)} initialized");
    }

    public LinePlotModelBuilder Rebuild(PlotModel plotModel)
    {
        _plotModel = plotModel;
        return this;
    }

    public PlotModel Build()
    {
        return _plotModel;
    }

    public LinePlotModelBuilder AddTemperatureSeries(IEnumerable<DataPoint> temperaturePoints)
    {
        var temperatureSeries = new LineSeries {Tag = PlotElementTags.SeriesTemperature};
        temperatureSeries.Points.AddRange(temperaturePoints);

        var yAxis = new LinearAxis {Tag = PlotElementTags.TemperatureYAxis};

        _plotModel.Series.Add(temperatureSeries);
        _plotModel.Axes.Add(yAxis);

        return this;
    }

    public LinePlotModelBuilder RemoveTemperatureSeries()
    {
        if (_plotModel.Series.Count == 0) throw new InvalidOperationException();

        var series = _plotModel.Series.First(s => s.Tag == PlotElementTags.SeriesTemperature);
        var yAxis = _plotModel.Axes.First(axis => axis.Tag == PlotElementTags.TemperatureYAxis);

        _plotModel.Series.Remove(series);
        _plotModel.Axes.Remove(yAxis);

        return this;
    }

    public LinePlotModelBuilder AddHumiditySeries(IEnumerable<DataPoint> humidityPoints)
    {
        var humiditySeries = new LineSeries {Tag = PlotElementTags.SeriesHumidity};
        humiditySeries.Points.AddRange(humidityPoints);

        var yAxis = new LinearAxis {Tag = PlotElementTags.HumidityYAxis};

        _plotModel.Series.Add(humiditySeries);
        _plotModel.Axes.Add(yAxis);

        return this;
    }

    public LinePlotModelBuilder RemoveHumiditySeries()
    {
        if (_plotModel.Series.Count == 0) throw new InvalidOperationException();

        var series = _plotModel.Series.First(s => s.Tag == PlotElementTags.SeriesHumidity);
        var yAxis = _plotModel.Axes.First(axis => axis.Tag == PlotElementTags.HumidityYAxis);

        _plotModel.Series.Remove(series);
        _plotModel.Axes.Remove(yAxis);

        return this;
    }

    public LinePlotModelBuilder AddPressureSeries(IEnumerable<DataPoint> pressurePoints)
    {
        var pressureSeries = new LineSeries {Tag = PlotElementTags.SeriesPressure};
        pressureSeries.Points.AddRange(pressurePoints);

        var yAxis = new LinearAxis {Tag = PlotElementTags.PressureYAxis};

        _plotModel.Series.Add(pressureSeries);
        _plotModel.Axes.Add(yAxis);

        return this;
    }

    public LinePlotModelBuilder RemovePressureSeries()
    {
        if (_plotModel.Series.Count == 0) throw new InvalidOperationException();

        var series = _plotModel.Series.First(s => s.Tag == PlotElementTags.SeriesPressure);
        var yAxis = _plotModel.Axes.First(axis => axis.Tag == PlotElementTags.PressureYAxis);

        _plotModel.Series.Remove(series);
        _plotModel.Axes.Remove(yAxis);

        return this;
    }

    public LinePlotModelBuilder AddDateTimeAxis()
    {
        _plotModel.Axes.Add(new DateTimeAxis {Tag = PlotElementTags.DateTimeAxis});

        return this;
    }

    public LinePlotModelBuilder Customize(IPlotCustomizer? plotCustomizer = null)
    {
        plotCustomizer ??= Locator.Current.GetService<IPlotCustomizer>()
                           ?? throw new NoServiceFound(nameof(IPlotCustomizer));
        plotCustomizer.Customize(_plotModel);

        return this;
    }
}