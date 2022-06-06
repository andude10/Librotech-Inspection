using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using LibrotechInspection.Core.Services.CsvPlotDataParser;
using Xunit;

namespace UnitTests.ParsersTests.ChartDataParsersTests;

public class CsvChartDataTests
{
    private const string TestDataFileName = @"testChartData.csv";
    private const string TestDataDirectory = @"TestData";

    private static string GetChartData()
    {
        var path = Path.GetFullPath(Path.Combine(TestDataDirectory, TestDataFileName));

        return File.ReadAllText(path);
    }

    [Fact]
    public async Task Parse_empty_data()
    {
        // Arrange
        var data = string.Empty;
        var parser = new CsvPlotDataParser();

        // Act & Assert
        await Assert.ThrowsAsync<ReaderException>(
            async () => await parser.ParseTemperatureAsync(data).ToListAsync());
    }

    [Fact]
    public async Task Parse_valid_temperature_data()
    {
        // Arrange
        var data = GetChartData();
        var parser = new CsvPlotDataParser();

        // Act
        var points = await parser.ParseTemperatureAsync(data).ToListAsync();

        // Assert
        Assert.NotEmpty(points);
    }

    [Fact]
    public async Task Parse_valid_humidity_data()
    {
        // Arrange
        var data = GetChartData();
        var parser = new CsvPlotDataParser();

        // Act
        var points = await parser.ParseHumidityAsync(data).ToListAsync();

        // Assert
        Assert.NotEmpty(points);
    }

    [Fact]
    public async Task Parse_not_included_data()
    {
        // Arrange
        var data = GetChartData();
        var parser = new CsvPlotDataParser();

        // Act
        var points = await parser.ParsePressureAsync(data).ToListAsync();

        // Assert
        Assert.Empty(points);
    }
}