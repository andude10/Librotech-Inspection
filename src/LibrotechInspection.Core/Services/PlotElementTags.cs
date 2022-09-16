namespace LibrotechInspection.Core.Services;

/// <summary>
///     PlotElementTags are the tags of all PlotModel elements.
///     Tags are used to find an element and customize it (by ChartCustomizer object)
/// </summary>
public static class PlotElementTags
{
    public static readonly object DateTimeAxis = nameof(DateTimeAxis);

    public static readonly object SeriesTemperature = nameof(SeriesTemperature);
    public static readonly object SeriesHumidity = nameof(SeriesHumidity);
    public static readonly object SeriesPressure = nameof(SeriesPressure);

    public static readonly object SeriesTemperatureMarked = nameof(SeriesTemperatureMarked);
    public static readonly object SeriesHumidityMarked = nameof(SeriesHumidityMarked);
    public static readonly object SeriesPressureMarked = nameof(SeriesPressureMarked);

    public static readonly object TemperatureYAxis = nameof(TemperatureYAxis);
    public static readonly object HumidityYAxis = nameof(HumidityYAxis);
    public static readonly object PressureYAxis = nameof(PressureYAxis);

    public static readonly object SeparatorLine = nameof(SeparatorLine);
}