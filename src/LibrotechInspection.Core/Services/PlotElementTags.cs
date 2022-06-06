namespace LibrotechInspection.Core.Services;

/// <summary>
///     PlotElementTags are the tags of all PlotModel elements.
///     Tags are used to find an element and customize it (by ChartCustomizer object)
/// </summary>
public static class PlotElementTags
{
    public static readonly object DateTimeAxis = "DateTimeAxis";

    public static readonly object SeriesTemperature = "SeriesTemperature";
    public static readonly object SeriesHumidity = "SeriesHumidity";
    public static readonly object SeriesPressure = "SeriesPressure";

    public static readonly object TemperatureYAxis = "TemperatureYAxis";
    public static readonly object HumidityYAxis = "HumidityYAxis";
    public static readonly object PressureYAxis = "PressureYAxis";
}