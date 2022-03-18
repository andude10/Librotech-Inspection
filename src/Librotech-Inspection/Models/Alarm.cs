using OxyPlot;

namespace Librotech_Inspection.Models;

/// <summary>
///     An alarm is an object containing information about an interval
///     on the chart that went beyond the set thresholds.
/// </summary>
public class Alarm
{
    public AlarmType AlarmType { get; set; }
    public DataPoint StartPoint { get; set; }
    public DataPoint LastPoint { get; set; }
}

/// <summary>
///     Alarm Type is the type of alarm (above the set threshold, below the set threshold)
/// </summary>
public enum AlarmType
{
    Higher,
    Lower
}