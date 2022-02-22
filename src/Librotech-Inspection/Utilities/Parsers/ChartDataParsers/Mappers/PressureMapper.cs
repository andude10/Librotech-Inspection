using CsvHelper.Configuration;
using Librotech_Inspection.Models;

namespace Librotech_Inspection.Utilities.Parsers.ChartDataParsers.Mappers;

public class PressureMapper : ClassMap<ChartPoint>
{
    public PressureMapper()
    {
        Map(p => p.X).Name("Дата/время");
        Map(p => p.Y).Name("Давление");
    }
}