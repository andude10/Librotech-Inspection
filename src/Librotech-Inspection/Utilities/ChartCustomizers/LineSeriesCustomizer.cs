using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Librotech_Inspection.Utilities.ChartCustomizers;

public class LineSeriesCustomizer : IChartCustomizer
{
    public void Customize(PlotModel plotModel)
    {
        CustomizeDateTimeAxis(plotModel);
        CustomizeSeries(plotModel);
        CustomizeYAxis(plotModel);
    }

    private void CustomizeDateTimeAxis(PlotModel plotModel)
    {
        var axis = plotModel.Axes.First(a => a.Tag == ChartElementTags.DateTimeAxis);

        if (axis == null) return;

        axis.Title = "Дата/Время";
        axis.Position = AxisPosition.Bottom;
        axis.StringFormat = "yyyy-MM-dd";
        axis.MajorStep = 50;
        axis.MajorGridlineStyle = LineStyle.Solid;
        axis.MinorGridlineStyle = LineStyle.Dot;
    }

    private void CustomizeSeries(PlotModel plotModel)
    {
        foreach (var t in plotModel.Series)
            t.TrackerFormatString = "{0}\nВремя: {2:yyyy-MM-dd HH:mm}\nЗначение: {4:0.0000}";

        if (plotModel.Series.FirstOrDefault(s =>
                s.Tag == ChartElementTags.LineSeriesTemperature) is LineSeries temperature)
        {
            temperature.Title = "Температура";
            temperature.Color = OxyColors.Red;
        }

        if (plotModel.Series.FirstOrDefault(s =>
                s.Tag == ChartElementTags.LineSeriesHumidity) is LineSeries humidity)
        {
            humidity.Title = "Влажность";
            humidity.Color = OxyColors.Blue;
        }

        if (plotModel.Series.FirstOrDefault(s =>
                s.Tag == ChartElementTags.LineSeriesPressure) is LineSeries pressure)
        {
            pressure.Title = "Давление";
            pressure.Color = OxyColors.Green;
        }
    }

    private void CustomizeYAxis(PlotModel plotModel)
    {
        var defAxis = plotModel.Axes.First();
        defAxis.MajorGridlineStyle = LineStyle.Solid;
        defAxis.MinorGridlineStyle = LineStyle.Dot;

        if (plotModel.Axes.FirstOrDefault(s =>
                s.Tag == ChartElementTags.TemperatureYAxis) is LinearAxis temperature)
        {
            temperature.Position = AxisPosition.Left;
            temperature.AxislineColor = OxyColors.Red;
            temperature.Maximum = 50;
            temperature.Minimum = -50;
        }

        if (plotModel.Axes.FirstOrDefault(s =>
                s.Tag == ChartElementTags.HumidityYAxis) is LinearAxis humidity)
        {
            humidity.Position = AxisPosition.Left;
            humidity.AxislineColor = OxyColors.Blue;
            humidity.Maximum = 100;
            humidity.Minimum = 0;
            humidity.PositionTier = 1;
        }

        if (plotModel.Axes.FirstOrDefault(s =>
                s.Tag == ChartElementTags.PressureYAxis) is LinearAxis pressure)
        {
            pressure.Position = AxisPosition.Left;
            pressure.AxislineColor = OxyColors.Green;
            pressure.Maximum = 50;
            pressure.Minimum = -50;
            pressure.PositionTier = 2;
        }
    }
}