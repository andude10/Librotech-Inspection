using System.IO;
using System.Threading.Tasks;
using LibrotechInspection.Core.Services.CsvFileParser;
using Xunit;

namespace LibrotechInspection.Core.Tests.ParsersTests.AllDataParsersTests;

public class CsvFileParserTests
{
    private const string TestDataFirst = "testFile.csv";
    private const string TestDataSecond = "testFile2.csv";
    private const string TestDataDirectory = "TestData";

    /// <summary>
    ///     Get path to data
    /// </summary>
    /// <returns></returns>
    private static string TestDataFirstCase()
    {
        return Path.GetFullPath(Path.Combine(TestDataDirectory, TestDataFirst));
    }

    /// <summary>
    ///     Get path to data
    /// </summary>
    /// <returns></returns>
    private static string TestDataSecondCase()
    {
        return Path.GetFullPath(Path.Combine(TestDataDirectory, TestDataSecond));
    }

    [Fact]
    public async Task Parse_valid_data_case_first()
    {
        // Arrange
        var path = TestDataFirstCase();
        var parser = new CsvFileParser();

        // Act
        var data = await parser.ParseAsync(path);

        // Assert
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Parse_valid_data_case_second()
    {
        // Arrange
        var path = TestDataSecondCase();
        var parser = new CsvFileParser();

        // Act
        var data = await parser.ParseAsync(path);

        // Assert
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Parse_with_invalid_path()
    {
        // Arrange
        const string path = "/invalid/path";
        var parser = new CsvFileParser();

        // Act
        var data = await parser.ParseAsync(path);

        // Assert
        Assert.True(data is null);
    }
}