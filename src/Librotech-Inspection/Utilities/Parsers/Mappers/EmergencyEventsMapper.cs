using CsvHelper.Configuration;
using Librotech_Inspection.Models;

namespace Librotech_Inspection.Utilities.Parsers.Mappers;

public sealed class EmergencyEventsMapper: ClassMap<EmergencyEvents>
{
    public EmergencyEventsMapper()
    {
        Map(a => a.Installation).Name("Установка");
        Map(a => a.Type).Name("Тип");
        Map(a => a.TimeAllowed).Name("Допустимое время");
        Map(a => a.TotalTime).Name("Общее время");
        Map(a => a.NumberOfAccidents).Name("Количество аварий");
        Map(a => a.Status).Name("Статус");
    }
}