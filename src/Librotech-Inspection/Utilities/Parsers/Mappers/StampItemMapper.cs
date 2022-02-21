using CsvHelper.Configuration;
using Librotech_Inspection.Models;

namespace Librotech_Inspection.Utilities.Parsers.Mappers;

public sealed class StampItemMapper: ClassMap<StampItem>
{
    public StampItemMapper()
    {
        Map(s => s.Name).Index(0);
        Map(s => s.Value).Index(1);
    }
}