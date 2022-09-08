using System;
using System.Linq;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace LibrotechInspection.Desktop.Services;

/// <summary>
///     The LinePlotCustomizer object customizes the entire visual part of the line chart.
/// </summary>
// TODO: everything is hardcoded, I'll think and rewrite
public class LinePlotCustomizer : IPlotCustomizer
{
    public void Customize(PlotModel? plotModel)
    {
        if (plotModel == null) throw new NullReferenceException();

        CustomizeDateTimeAxis(plotModel);
        CustomizeSeries(plotModel);
        CustomizeMarkedSeries(plotModel);
        CustomizeYAxis(plotModel);
    }

    private void CustomizeDateTimeAxis(PlotModel plotModel)
    {
        var axis = plotModel.Axes.First(a => a.Tag == PlotElementTags.DateTimeAxis);

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
        {
            s.TrackerFormatString = "{0}\nВремя: {2:yyyy-MM-dd HH:mm}\nЗначение: {4:0.0}";
            s.SelectionMode = SelectionMode.Multiple;
        }

        if (plotModel.Series.FirstOrDefault(s =>
                s.Tag == PlotElementTags.SeriesTemperature) is LineSeries t)
        {
            t.Title = "Температура";
            t.Color = OxyColors.OrangeRed;
        }

        if (plotModel.Series.FirstOrDefault(s =>
                s.Tag == PlotElementTags.SeriesHumidity) is LineSeries h)
        {
            h.Title = "Влажность";
            h.Color = OxyColors.Blue;
        }

        if (plotModel.Series.FirstOrDefault(s =>
                s.Tag == PlotElementTags.SeriesPressure) is LineSeries p)
        {
            p.Title = "Давление";
            p.Color = OxyColors.Green;
        }
    }

    private void CustomizeMarkedSeries(PlotModel plotModel)
    {
        if (plotModel.Series.FirstOrDefault(s =>
                s.Tag == PlotElementTags.SeriesTemperatureMarked) is LineSeries t)
        {
            t.MarkerType = MarkerType.Circle;
            t.MarkerSize = 7;
            t.Color = OxyColors.Red;
            t.StrokeThickness = 0;
        }

        if (plotModel.Series.FirstOrDefault(s =>
                s.Tag == PlotElementTags.SeriesHumidityMarked) is LineSeries h)
        {
            h.MarkerType = MarkerType.Circle;
            h.MarkerSize = 7;
            h.Color = OxyColors.MidnightBlue;
            h.StrokeThickness = 0;
        }

        if (plotModel.Series.FirstOrDefault(s =>
                s.Tag == PlotElementTags.SeriesPressureMarked) is LineSeries p)
        {
            p.MarkerType = MarkerType.Circle;
            p.MarkerSize = 7;
            p.Color = OxyColors.DarkGreen;
            p.StrokeThickness = 0;
        }
    }

    private void CustomizeYAxis(PlotModel plotModel)
    {
        if (plotModel.Axes.FirstOrDefault(s =>
                s.Tag == PlotElementTags.TemperatureYAxis) is LinearAxis t)
        {
            var tSeries = plotModel.Series.First(s => s.Tag == PlotElementTags.SeriesTemperature)
                as LineSeries;

            t.Position = AxisPosition.Left;
            t.AxislineColor = OxyColors.Red;

            var maxValue = tSeries.Points.MaxBy(x => x.Y).Y;
            t.Maximum = maxValue + maxValue * 1.4;

            var minValue = tSeries.Points.MinBy(x => x.Y).Y;
            t.Minimum = minValue - minValue * 0.2;
        }

        if (plotModel.Axes.FirstOrDefault(s =>
                s.Tag == PlotElementTags.HumidityYAxis) is LinearAxis h)
        {
            var hSeries = plotModel.Series.First(s => s.Tag == PlotElementTags.SeriesHumidity)
                as LineSeries;

            h.Position = AxisPosition.Left;
            h.AxislineColor = OxyColors.Blue;

            var maxValue = hSeries.Points.MaxBy(x => x.Y).Y;
            h.Maximum = maxValue + maxValue * 1.4;

            var minValue = hSeries.Points.MinBy(x => x.Y).Y;
            h.Minimum = minValue - minValue * 0.2;

            h.PositionTier = 1;
        }

        if (plotModel.Axes.FirstOrDefault(s =>
                s.Tag == PlotElementTags.PressureYAxis) is LinearAxis p)
        {
            var pSeries = plotModel.Series.First(s => s.Tag == PlotElementTags.SeriesPressure)
                as LineSeries;

            p.Position = AxisPosition.Left;
            p.AxislineColor = OxyColors.Green;

            var maxValue = pSeries.Points.MaxBy(x => x.Y).Y;
            p.Maximum = maxValue + maxValue * 1.4;

            var minValue = pSeries.Points.MinBy(x => x.Y).Y;
            p.Minimum = minValue - minValue * 0.2;

            p.PositionTier = 2;
        }
    }
}