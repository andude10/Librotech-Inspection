using OxyPlot;

namespace LibrotechInspection.Desktop.Models;

public class SelectedDataPoint
{
    public SelectedDataPoint(DataPoint point, Element parentElement)
    {
        Point = point;
        ParentElement = parentElement;
    }

    public DataPoint Point { get; }
    public Element ParentElement { get; }
}