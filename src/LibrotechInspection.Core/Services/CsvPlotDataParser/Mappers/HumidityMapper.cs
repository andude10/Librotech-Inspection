using CsvHelper.Configuration;
using LibrotechInspection.Core.Models;

namespace LibrotechInspection.Core.Services.CsvPlotDataParser.Mappers;

public class HumidityMapper : ClassMap<PlotPoint>
{
    public HumidityMapper()
    {
        Map(p => p.X).Name("Дата/время");
        Map(p => p.Y).Name("Влажность");
    }
}