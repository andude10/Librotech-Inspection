using CsvHelper.Configuration;
using LibrotechInspection.Core.Models;

namespace LibrotechInspection.Core.Services.CsvPlotDataParser.Mappers;

public sealed class TemperatureMapper : ClassMap<PlotPoint>
{
    public TemperatureMapper()
    {
        Map(p => p.X).Name("Дата/время");
        Map(p => p.Y).Name("Температура");
    }
}