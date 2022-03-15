namespace Librotech_Inspection.Utilities.DataDecorators.Representatives;

/// <summary>
///     ShortSummary represents the data of a short summary
/// </summary>
public struct ShortSummary
{
    public string SessionId { get; set; }
    public string SessionStart { get; set; }
    public string SessionEnd { get; set; }
    public string TotalDuration { get; set; }
}