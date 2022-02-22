using CsvHelper.Configuration;
using Librotech_Inspection.Models;

namespace Librotech_Inspection.Utilities.Parsers.ChartDataParsers.Mappers;

public sealed class TemperatureMapper : ClassMap<ChartPoint>
{
    public TemperatureMapper()
    {
        Map(p => p.X).Name("Дата/время");
        Map(p => p.Y).Name("Температура");
    }
}