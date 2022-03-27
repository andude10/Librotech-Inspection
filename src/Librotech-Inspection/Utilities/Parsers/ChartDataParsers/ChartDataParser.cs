using System.Collections.Generic;
using Librotech_Inspection.Utilities.Parsers.ChartDataParsers.CsvFile;

namespace Librotech_Inspection.Utilities.Parsers.ChartDataParsers;

public abstract class ChartDataParser
{
    public abstract IAsyncEnumerable<ChartPoint> ParseTemperatureAsync(string data);
    public abstract IAsyncEnumerable<ChartPoint> ParseHumidityAsync(string data);
    public abstract IAsyncEnumerable<ChartPoint> ParsePressureAsync(string data);
}