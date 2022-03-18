using CsvHelper.Configuration;
using Librotech_Inspection.Models;

namespace Librotech_Inspection.Utilities.Parsers.FileParsers.Mappers;

public sealed class EmergencyEventsSettingsMapper : ClassMap<EmergencyEventsSettings>
{
    public EmergencyEventsSettingsMapper()
    {
        Map(a => a.Installation).Name("Установка");
        Map(a => a.Type).Name("Тип");
        Map(a => a.TimeAllowed).Name("Допустимое время");
        Map(a => a.TotalTime).Name("Общее время");
        Map(a => a.NumberOfAccidents).Name("Количество аварий");
        Map(a => a.Status).Name("Статус");
    }
}