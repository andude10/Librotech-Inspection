using System.Linq;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Services;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace LibrotechInspection.Desktop.Services;

/// <summary>
///     The LinePlotCustomizer object customizes the entire visual part of the line chart.
/// </summary>
// TODO: everything is hardcoded, I'll think and rewrite
public class LinePlotCustomizer : IPlotCustomizer
{
    public void Customize(PlotModel plotModel)
    {
        CustomizeDateTimeAxis(plotModel);
        CustomizeSeries(plotModel);
        CustomizeMarkedSeries(plotModel);
        CustomizeYAxis(plotModel);
        CustomizeAnnotations(plotModel);
    }

    private void CustomizeDateTimeAxis(PlotModel plotModel)
    {
        if (plotModel.Axes.Count == 0) return;

        var axis = plotModel.Axes.First(a => a.Tag == PlotElementTags.DateTimeAxis);

        if (axis == null) return;

        axis.Title = "Дата/Время";
        axis.Position = AxisPosition.Bottom;
        axis.StringFormat = "yyyy.MM.dd hh:mm";
        axis.MajorGridlineStyle = LineStyle.Solid;
        axis.MinorGridlineStyle = LineStyle.Dot;
    }

    private void CustomizeSeries(PlotModel plotModel)
    {
        if (plotModel.Series.Count == 0) return;

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
        if (plotModel.Series.Count == 0) return;

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
        if (plotModel.Axes.Count == 0) return;

        if (plotModel.Axes.FirstOrDefault(s =>
                s.Tag == PlotElementTags.TemperatureYAxis) is LinearAxis t)
        {
            t.Title = "Температура";
            t.TitleFontWeight = 800D;
            t.Unit = "℃";

            t.Position = AxisPosition.Left;
            t.PositionTier = 0;

            t.TitleColor = OxyColors.Red;
            t.TextColor = OxyColors.Red;
            t.AxislineColor = OxyColors.Red;

            t.AxislineThickness = 3;
            t.AxislineStyle = LineStyle.Solid;

            t.MaximumPadding = 0.02;
            t.MinimumPadding = 0.05;
        }

        if (plotModel.Axes.FirstOrDefault(s =>
                s.Tag == PlotElementTags.HumidityYAxis) is LinearAxis h)
        {
            h.Title = "Влажность";
            h.TitleFontWeight = 800D;
            h.Unit = "%";

            h.Position = AxisPosition.Left;
            h.PositionTier = 1;

            h.AxislineColor = OxyColors.MidnightBlue;
            h.TitleColor = OxyColors.MidnightBlue;
            h.TextColor = OxyColors.MidnightBlue;

            h.AxislineThickness = 3;
            h.AxislineStyle = LineStyle.Solid;

            h.MaximumPadding = 0.07;
            h.MinimumPadding = 0.02;
        }

        if (plotModel.Axes.FirstOrDefault(s =>
                s.Tag == PlotElementTags.PressureYAxis) is LinearAxis p)
        {
            var pSeries = plotModel.Series.First(s => s.Tag == PlotElementTags.SeriesPressure)
                as LineSeries;

            p.Title = "Давление";
            p.Unit = "Па";
            p.TitleFontWeight = 800D;

            p.Position = AxisPosition.Left;
            p.AxislineColor = OxyColors.Green;
            p.TextColor = OxyColors.Green;
            p.AxislineThickness = 3;
            p.AxislineStyle = LineStyle.Solid;
            p.PositionTier = 2;

            var maxValue = pSeries.Points.MaxBy(x => x.Y).Y;
            p.Maximum = maxValue + maxValue * 0.4;

            var minValue = pSeries.Points.MinBy(x => x.Y).Y;
            p.Minimum = minValue - minValue * 0.4;
        }
    }

    private void CustomizeAnnotations(PlotModel plotModel)
    {
        if (plotModel.Annotations.Count == 0) return;

        var separatorLines = plotModel.Annotations.Where(a => a.Tag == PlotElementTags.SeparatorLine)
            .OfType<LineAnnotation>().ToArray();

        if (separatorLines.Length == 0) return;

        foreach (var line in separatorLines)
        {
            line.StrokeThickness = 3;
            line.Color = OxyColors.Black;
        }
    }
}