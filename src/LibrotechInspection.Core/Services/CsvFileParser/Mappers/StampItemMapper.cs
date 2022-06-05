using CsvHelper.Configuration;
using LibrotechInspection.Core.Models;

namespace LibrotechInspection.Core.Services.CsvFileParser.Mappers;

public sealed class StampItemMapper : ClassMap<StampItem>
{
    public StampItemMapper()
    {
        Map(s => s.Name).Index(0);
        Map(s => s.Value).Index(1);
    }
}