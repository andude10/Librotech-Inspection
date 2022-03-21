using CsvHelper.Configuration;

namespace Librotech_Inspection.Utilities.Parsers.ChartDataParsers.CsvFile.Mappers;

public class HumidityMapper : ClassMap<ChartPoint>
{
    public HumidityMapper()
    {
        Map(p => p.X).Name("Дата/время");
        Map(p => p.Y).Name("Влажность");
    }
}