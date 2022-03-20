using System.Collections.Generic;

namespace Librotech_Inspection.Models;

public interface IReadableData
{
    string FileName { get; }
    string ChartData { get; }
    IEnumerable<DeviceSpecification>? DeviceSpecifications { get; }
    IEnumerable<EmergencyEventsSettings>? EmergencyEventsSettings { get; }
    IEnumerable<Stamp>? Stamps { get; }
}