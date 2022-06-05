namespace LibrotechInspection.Core.Models.Record;

public abstract class Record
{
    public string PlotData { get; init; }
    public IEnumerable<DeviceCharacteristic>? DeviceSpecifications { get; init; }
    public IEnumerable<EmergencyEventsSettings>? EmergencyEventsSettings { get; init; }
    public IEnumerable<Stamp>? Stamps { get; init; }
}