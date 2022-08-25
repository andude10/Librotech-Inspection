using LibrotechInspection.Core.Services;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace LibrotechInspection.Desktop.Services;

public class PlotElementProvider : IPlotElementProvider
{
    public LineSeries GetTemperatureSeries()
    {
        return new() {Tag = PlotElementTags.SeriesTemperature};
    }

    public LineSeries GetHumiditySeries()
    {
        return new() {Tag = PlotElementTags.SeriesHumidity};
    }

    public LineSeries GetPressureSeries()
    {
        return new() {Tag = PlotElementTags.SeriesPressure};
    }

    public LinearAxis GetTemperatureYAxis()
    {
        return new() {Tag = PlotElementTags.TemperatureYAxis};
    }

    public LinearAxis GetHumidityYAxis()
    {
        return new() {Tag = PlotElementTags.HumidityYAxis};
    }

    public LinearAxis GetPressureYAxis()
    {
        return new() {Tag = PlotElementTags.PressureYAxis};
    }

    public DateTimeAxis GetXAxis()
    {
        return new() {Tag = PlotElementTags.DateTimeAxis};
    }
}