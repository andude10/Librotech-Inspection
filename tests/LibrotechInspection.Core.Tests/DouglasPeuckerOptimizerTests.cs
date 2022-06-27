using System.Collections.Generic;
using System.Threading.Tasks;
using LibrotechInspection.Core.Services;
using OxyPlot;
using Xunit;

namespace LibrotechInspection.Core.Tests;

public class DouglasPeuckerOptimizerTests
{
    private List<DataPoint> GetPoints()
    {
        var points = new List<DataPoint>
        {
            new(0, 0),
            new(1, 0),
            new(2, -0.1),
            new(3, 5),
            new(4, 6),
            new(5, 7),
            new(6, 8.1),
            new(7, 9),
            new(8, 9),
            new(9, 9)
        };
        return points;
    }

    [Fact]
    public async Task Try_optimize()
    {
        // Arrange
        var optimizedPoints = GetPoints();
        var oldPoints = GetPoints();
        var optimizer = new DouglasPeuckerOptimizer();

        // Act
        await optimizer.OptimizeAsync(optimizedPoints);

        // Assert
        Assert.True(optimizedPoints.Count > 2 && oldPoints.Count > optimizedPoints.Count);
    }
}