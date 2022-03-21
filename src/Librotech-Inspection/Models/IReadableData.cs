using System.Collections.Generic;

namespace Librotech_Inspection.Models;

/// <summary>
///     IReadableData interface that represents read-only data
/// </summary>
public interface IReadableData
{
    string FileName { get; }
    string ChartData { get; }
    IEnumerable<DeviceSpecification>? DeviceSpecifications { get; }
    IEnumerable<EmergencyEventsSettings>? EmergencyEventsSettings { get; }
    IEnumerable<Stamp>? Stamps { get; }
}