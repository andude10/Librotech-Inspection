namespace LibrotechInspection.Core.Models;

public struct EmergencyEventsSettings
{
    public string Installation { get; set; }
    public string Type { get; set; }
    public string TimeAllowed { get; set; }
    public string TotalTime { get; set; }
    public string NumberOfAccidents { get; set; }
    public string Status { get; set; }
}