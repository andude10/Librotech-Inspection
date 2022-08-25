using OxyPlot;

namespace LibrotechInspection.Desktop.Utilities.Json;

/*
 TODO: I haven't come up with anything better yet to deserialize a DataPoint from the
         'OxyPlot' library (when deserializing, X and Y will be zero). 
         Perhaps I need to write a converter, or maybe serialize DataPoint is bad idea?
 */
public class SerializableDataPoint : IDataPointProvider
{
    public SerializableDataPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; set; }
    public double Y { get; set; }

    public DataPoint GetDataPoint()
    {
        return new DataPoint(X, Y);
    }
}