using System;

namespace Librotech_Inspection.Utilities.Parsers.ChartDataParsers.CsvFile;

/// <summary>
///     ChartPoint used by parsers (and mappers)
///     to parse an X value as a DateTime
/// </summary>
public class ChartPoint
{
    public DateTime X { get; set; }
    public double Y { get; set; }
}