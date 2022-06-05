namespace LibrotechInspection.Core.Models;

/// <summary>
///     Represents the device characteristic name and value (from a file)
///     For example: (name)Logger type: (value)SX100-H BLR
/// </summary>
public struct DeviceCharacteristic
{
    public string Name { get; set; }
    public string Value { get; set; }
}