using OxyPlot;

namespace LibrotechInspection.Core.Interfaces;

public interface ILinePlotOptimizer
{
    public Task OptimizeAsync(List<DataPoint> points);
}