using OxyPlot.Axes;
using OxyPlot.Series;

namespace LibrotechInspection.Desktop.Services;

public interface IPlotElementProvider
{
    public LineSeries GetTemperatureSeries();
    public LineSeries GetHumiditySeries();
    public LineSeries GetPressureSeries();
    public LinearAxis GetTemperatureYAxis();
    public LinearAxis GetHumidityYAxis();
    public LinearAxis GetPressureYAxis();
    public DateTimeAxis GetXAxis();
}