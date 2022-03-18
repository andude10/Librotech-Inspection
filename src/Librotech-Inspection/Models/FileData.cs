using System.Collections.Generic;

namespace Librotech_Inspection.Models;

/// <summary>
///     FileData represents data received from a file.
/// </summary>
public class FileData
{
    public string FileName { get; set; }
    public string ChartData { get; set; }
    public List<DeviceSpecification>? DeviceSpecifications { get; set; } = new();
    public List<EmergencyEventsSettings>? EmergencyEventsSettings { get; set; } = new();
    public List<Stamp>? Stamps { get; set; } = new();
}