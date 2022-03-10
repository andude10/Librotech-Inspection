using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Librotech_Inspection.Utilities.ChartCustomizers;

public class LineChartCustomizer : ChartCustomizer
{
    public override void Customize(PlotModel plotModel)
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
        axis.MajorGridlineStyle = LineStyle.Solid;
        axis.MinorGridlineStyle = LineStyle.Dot;
    }

    private void CustomizeSeries(PlotModel plotModel)
    {
        foreach (var s in plotModel.Series)
            s.TrackerFormatString = "{0}\nВремя: {2:yyyy-MM-dd HH:mm}\nЗначение: {4:0.0000}";

        if (plotModel.Series.FirstOrDefault(s =>
                s.Tag == ChartElementTags.LineSeriesTemperature) is LineSeries t)
        {
            t.Title = "Температура";
            t.Color = OxyColors.Red;
        }

        if (plotModel.Series.FirstOrDefault(s =>
                s.Tag == ChartElementTags.LineSeriesHumidity) is LineSeries h)
        {
            h.Title = "Влажность";
            h.Color = OxyColors.Blue;
        }

        if (plotModel.Series.FirstOrDefault(s =>
                s.Tag == ChartElementTags.LineSeriesPressure) is LineSeries p)
        {
            p.Title = "Давление";
            p.Color = OxyColors.Green;
        }
    }

    private void CustomizeYAxis(PlotModel plotModel)
    {
        var defAxis = plotModel.Axes.First();
        defAxis.MajorGridlineStyle = LineStyle.Solid;
        defAxis.MinorGridlineStyle = LineStyle.Dot;

        if (plotModel.Axes.FirstOrDefault(s =>
                s.Tag == ChartElementTags.TemperatureYAxis) is LinearAxis t)
        {
            var tSeries = plotModel.Series.First(s => s.Tag == ChartElementTags.LineSeriesTemperature)
                as LineSeries;

            t.Position = AxisPosition.Left;
            t.AxislineColor = OxyColors.Red;
            t.Maximum = tSeries.MaxY + tSeries.MaxY * 0.4;
            t.Minimum = tSeries.MinY - tSeries.MinY * 0.4;
        }

        if (plotModel.Axes.FirstOrDefault(s =>
                s.Tag == ChartElementTags.HumidityYAxis) is LinearAxis h)
        {
            var hSeries = plotModel.Series.First(s => s.Tag == ChartElementTags.LineSeriesHumidity)
                as LineSeries;

            h.Position = AxisPosition.Left;
            h.AxislineColor = OxyColors.Blue;
            h.Maximum = hSeries.MaxY + hSeries.MaxY * 0.4;
            h.Minimum = hSeries.MinY - hSeries.MinY * 0.4;
            h.PositionTier = 1;
        }

        if (plotModel.Axes.FirstOrDefault(s =>
                s.Tag == ChartElementTags.PressureYAxis) is LinearAxis p)
        {
            var pSeries = plotModel.Series.First(s => s.Tag == ChartElementTags.LineSeriesPressure)
                as LineSeries;

            p.Position = AxisPosition.Left;
            p.AxislineColor = OxyColors.Green;
            p.Maximum = pSeries.MaxY + pSeries.MaxY * 0.4;
            p.Minimum = pSeries.MinY - pSeries.MinY * 0.4;
            p.PositionTier = 2;
        }
    }
}