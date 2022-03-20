using System.Collections.Generic;

namespace Librotech_Inspection.Models;

/// <summary>
///     Data represents data received from a file.
/// </summary>
public class Data : IReadableData
{
    public string FileName { get; set; }
    public string ChartData { get; set; }
    public IEnumerable<DeviceSpecification>? DeviceSpecifications { get; set; }
    public IEnumerable<EmergencyEventsSettings>? EmergencyEventsSettings { get; set; }
    public IEnumerable<Stamp>? Stamps { get; set; }
}