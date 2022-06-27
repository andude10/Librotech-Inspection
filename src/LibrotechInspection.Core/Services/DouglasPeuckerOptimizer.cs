using LibrotechInspection.Core.Interfaces;
using OxyPlot;

namespace LibrotechInspection.Core.Services;

/// <summary>
///     The optimizer which uses the Ramer–Douglas–Peucker algorithm to optimize plot series
/// </summary>
public class DouglasPeuckerOptimizer : ILinePlotOptimizer
{
    public async Task OptimizeAsync(List<DataPoint> points)
    {
        var result = new List<DataPoint>();

        // For the exact operation of the algorithm, we convert the values of the points into integers
        await Task.Run(() => ConvertToIntegerValues(points));

        await Task.Run(() => SimplifyPoints(points, 0.03, result));

        // Converting the values back 
        await Task.Run(() => ConvertToRationalPoints(result));

        points.Clear();
        points.AddRange(result);
    }

    private static void ConvertToIntegerValues(List<DataPoint> points)
    {
        var end = points.Count - 1;
        for (var i = 0; i < end; i++)
            points[i] = new DataPoint(Math.Truncate(points[i].X * 100000000000),
                Math.Truncate(points[i].Y * 10));
    }

    private static void ConvertToRationalPoints(List<DataPoint> points)
    {
        var end = points.Count - 1;
        for (var i = 0; i < end; i++)
            points[i] = new DataPoint(points[i].X / 100000000000,
                points[i].Y / 10);
    }

    private static void SimplifyPoints(List<DataPoint> points, double epsilon, List<DataPoint> output)
    {
        if (points.Count < 2) throw new ArgumentOutOfRangeException(nameof(points));

        // Find the point with the maximum distance
        var maxDistance = 0.0;
        var index = 0;
        var end = points.Count - 1;
        for (var i = 1; i < end; ++i)
        {
            var d = PerpendicularDistance(points[i], points[0], points[end]);

            if (!(d > maxDistance)) continue;

            index = i;
            maxDistance = d;
        }

        // If max distance is greater than epsilon, recursively simplify
        if (maxDistance > epsilon)
        {
            var results1 = new List<DataPoint>();
            var results2 = new List<DataPoint>();

            var firstLine = points.Take(index + 1).ToList();
            var lastLine = points.Skip(index).ToList();

            // Recursive call
            SimplifyPoints(firstLine, epsilon, results1);
            SimplifyPoints(lastLine, epsilon, results2);

            // Build the result list
            output.AddRange(results1.Take(results1.Count - 1));
            output.AddRange(results2);
            if (output.Count < 2) throw new Exception("Problem assembling output");
        }
        else
        {
            output.Clear();
            output.Add(points[0]);
            output.Add(points[^1]);
        }
    }

    private static double PerpendicularDistance(DataPoint point, DataPoint lineStart, DataPoint lineEnd)
    {
        var dx = lineEnd.X - lineStart.X;
        var dy = lineEnd.Y - lineStart.Y;

        // Normalize
        var mag = Math.Sqrt(dx * dx + dy * dy);
        if (mag > 0.0)
        {
            dx /= mag;
            dy /= mag;
        }

        var pvx = point.X - lineStart.X;
        var pvy = point.Y - lineStart.Y;

        var pvdot = dx * pvx + dy * pvy;

        var ax = pvx - pvdot * dx;
        var ay = pvy - pvdot * dy;

        return Math.Sqrt(ax * ax + ay * ay);
    }
}