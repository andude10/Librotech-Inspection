using CsvHelper.Configuration;
using LibrotechInspection.Core.Models;

namespace LibrotechInspection.Core.Services.CsvPlotDataParser.Mappers;

public class PressureMapper : ClassMap<PlotPoint>
{
    public PressureMapper()
    {
        Map(p => p.X).Name("Дата/время");
        Map(p => p.Y).Name("Давление");
    }
}