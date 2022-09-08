namespace LibrotechInspection.Desktop.Utilities.DataDecorators.Presenters;

public class SelectedPointOnPlotInfo
{
    public SelectedPointOnPlotInfo(string date, string value, string type)
    {
        Date = date;
        Value = value;
        Type = type;
    }

    public string Date { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }
}