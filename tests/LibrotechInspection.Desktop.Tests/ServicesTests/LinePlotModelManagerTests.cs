using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using FluentAssertions.Execution;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.Tests.Utilities;
using OxyPlot;
using Xunit;

namespace LibrotechInspection.Desktop.Tests.ServicesTests;

public class LinePlotModelManagerTests
{
    [Fact]
    public void Should_serialize_the_points_of_all_series()
    {
        // Arrange
        TestSetupHelper.RegisterServices();
        var manager = new LinePlotModelManager();
        var testPoint = new DataPoint(5, 10);
        manager.TemperatureSeries.Points.Add(testPoint);
        manager.HumiditySeries.Points.Add(testPoint);
        manager.PressureSeries.Points.Add(testPoint);

        manager.TemperatureMarkedSeries.Points.Add(testPoint);
        manager.HumidityMarkedSeries.Points.Add(testPoint);
        manager.PressureMarkedSeries.Points.Add(testPoint);

        // Act
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };
        var json = JsonSerializer.Serialize(manager, options);
        var modelAfter = JsonSerializer.Deserialize<LinePlotModelManager>(json, options);

        // Assert
        using (new AssertionScope())
        {
            modelAfter.Should().NotBeNull();
            modelAfter?.TemperatureSeries.Points.Should().NotBeEmpty();
            modelAfter?.HumiditySeries.Points.Should().NotBeEmpty();
            modelAfter?.PressureSeries.Points.Should().NotBeEmpty();

            modelAfter?.TemperatureMarkedSeries.Points.Should().NotBeEmpty();
            modelAfter?.HumidityMarkedSeries.Points.Should().NotBeEmpty();
            modelAfter?.PressureMarkedSeries.Points.Should().NotBeEmpty();
        }
    }
}