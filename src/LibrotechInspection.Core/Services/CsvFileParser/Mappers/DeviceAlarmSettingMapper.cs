using CsvHelper.Configuration;
using LibrotechInspection.Core.Models;

namespace LibrotechInspection.Core.Services.CsvFileParser.Mappers;

public sealed class DeviceAlarmSettingMapper : ClassMap<DeviceAlarmSetting>
{
    public DeviceAlarmSettingMapper()
    {
        Map(a => a.Installation).Name("Установка");
        Map(a => a.Type).Name("Тип");
        Map(a => a.TimeAllowed).Name("Допустимое время");
        Map(a => a.TotalTime).Name("Общее время");
        Map(a => a.NumberOfAccidents).Name("Количество аварий");
        Map(a => a.Status).Name("Статус");
    }
}