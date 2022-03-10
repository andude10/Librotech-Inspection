namespace Librotech_Inspection.Utilities;

/// <summary>
///     ChartElementTags are the tags of all PlotModel elements.
///     Tags are used to find an element and customize it (by ChartCustomizer object)
/// </summary>
public static class ChartElementTags
{
    public static readonly object DateTimeAxis = "DateTimeAxis";

    public static readonly object LineSeriesTemperature = "LineSeriesTemperature";
    public static readonly object LineSeriesHumidity = "LineSeriesHumidity";
    public static readonly object LineSeriesPressure = "LineSeriesPressure";

    public static readonly object TemperatureYAxis = "TemperatureYAxis";
    public static readonly object HumidityYAxis = "HumidityYAxis";
    public static readonly object PressureYAxis = "PressureYAxis";
}