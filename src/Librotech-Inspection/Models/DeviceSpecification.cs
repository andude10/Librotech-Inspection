namespace Librotech_Inspection.Models;

/// <summary>
/// Represents the device specification name and value (from a file)
/// For example: (name)Logger type: (value)SX100-H BLR
/// </summary>
public class DeviceSpecification
{
    public string Name { get; set; }
    public string Value { get; set; }
}