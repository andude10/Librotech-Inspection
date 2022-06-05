using CsvHelper.Configuration;
using LibrotechInspection.Core.Models;

namespace LibrotechInspection.Core.Services.CsvFileParser.Mappers;

public sealed class DeviceSpecificationMapper : ClassMap<DeviceCharacteristic>
{
    public DeviceSpecificationMapper()
    {
        Map(s => s.Name).Index(0);
        Map(s => s.Value).Index(1);
    }
}