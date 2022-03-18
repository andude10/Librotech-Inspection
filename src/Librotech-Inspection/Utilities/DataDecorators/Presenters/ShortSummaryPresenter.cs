namespace Librotech_Inspection.Utilities.DataDecorators.Presenters;

/// <summary>
///     ShortSummaryPresenter represents the data of a short summary
/// </summary>
public struct ShortSummaryPresenter
{
    public string SessionId { get; set; }
    public string SessionStart { get; set; }
    public string SessionEnd { get; set; }
    public string TotalDuration { get; set; }
}