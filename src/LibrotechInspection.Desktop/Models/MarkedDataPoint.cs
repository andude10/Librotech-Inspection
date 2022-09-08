using OxyPlot;

namespace LibrotechInspection.Desktop.Models;

public class MarkedDataPoint : IDataPointProvider
{
    public MarkedDataPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; set; }
    public double Y { get; set; }
    public string? Description { get; set; }

    public DataPoint GetDataPoint()
    {
        return new DataPoint(X, Y);
    }
}