namespace LibrotechInspection.Core.Models;

/// <summary>
///     PlotPoint used by parsers (and mappers)
///     to parse an X value as a DateTime
///</summary>
public class PlotPoint
{
    public DateTime X { get; set; }
    public double Y { get; set; }
}