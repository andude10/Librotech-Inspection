using CsvHelper.Configuration;

namespace Librotech_Inspection.Utilities.Parsers.ChartDataParsers.CsvFile.Mappers;

public class PressureMapper : ClassMap<ChartPoint>
{
    public PressureMapper()
    {
        Map(p => p.X).Name("Дата/время");
        Map(p => p.Y).Name("Давление");
    }
}