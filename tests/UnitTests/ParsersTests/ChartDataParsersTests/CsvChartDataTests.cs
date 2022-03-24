using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Librotech_Inspection.Utilities.Parsers.ChartDataParsers.CsvFile;
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

        // Act & Assert
        await Assert.ThrowsAsync<ReaderException>(
            async () => await CsvChartDataParser.ParseTemperatureAsync(data).ToListAsync());
    }

    [Fact]
    public async Task Parse_valid_temperature_data()
    {
        // Arrange
        var data = GetChartData();

        // Act
        var points = await CsvChartDataParser.ParseTemperatureAsync(data).ToListAsync();

        // Assert
        Assert.NotEmpty(points);
    }

    [Fact]
    public async Task Parse_valid_humidity_data()
    {
        // Arrange
        var data = GetChartData();

        // Act
        var points = await CsvChartDataParser.ParseHumidityAsync(data).ToListAsync();

        // Assert
        Assert.NotEmpty(points);
    }

    [Fact]
    public async Task Parse_not_included_data()
    {
        // Arrange
        var data = GetChartData();

        // Act
        var points = await CsvChartDataParser.ParsePressureAsync(data).ToListAsync();

        // Assert
        Assert.Empty(points);
    }
}