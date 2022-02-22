using CsvHelper.Configuration;
using Librotech_Inspection.Models;

namespace Librotech_Inspection.Utilities.Parsers.ChartDataParsers.Mappers;

public class HumidityMapper : ClassMap<ChartPoint>
{
    public HumidityMapper()
    {
        Map(p => p.X).Name("Дата/время");
        Map(p => p.Y).Name("Влажность");
    }
}