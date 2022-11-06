using System.Text.Json.Serialization;
using LibrotechInspection.Core.Utilities.JsonConverters;

namespace LibrotechInspection.Core.Models.Record;

[JsonConverter(typeof(RecordConverter))]
public abstract class Record
{
    public string? Name { get; set; }
    public abstract string RecordType { get; }
    public string PlotData { get; init; }
    public IEnumerable<DeviceCharacteristic>? DeviceSpecifications { get; init; }
    public IEnumerable<EmergencyEventsSettings>? EmergencyEventsSettings { get; init; }
    public IEnumerable<Stamp>? Stamps { get; init; }
}