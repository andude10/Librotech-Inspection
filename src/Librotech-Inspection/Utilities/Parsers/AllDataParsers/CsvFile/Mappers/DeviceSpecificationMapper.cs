using CsvHelper.Configuration;
using Librotech_Inspection.Models;

namespace Librotech_Inspection.Utilities.Parsers.AllDataParsers.CsvFile.Mappers;

public sealed class DeviceSpecificationMapper : ClassMap<DeviceSpecification>
{
    public DeviceSpecificationMapper()
    {
        Map(s => s.Name).Index(0);
        Map(s => s.Value).Index(1);
    }
}