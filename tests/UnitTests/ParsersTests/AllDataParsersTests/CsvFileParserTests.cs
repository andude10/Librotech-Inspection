using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Librotech_Inspection.Utilities.Parsers.AllDataParsers.CsvFile;
using Xunit;

namespace UnitTests.ParsersTests.AllDataParsersTests;

public class CsvFileParserTests
{
    private const string TestFileName = @"testFile.csv";
    private const string TestDataDirectory = @"TestData";
    private const int CodePage = 1251;

    private static string GetData()
    {
        var path = Path.GetFullPath(Path.Combine(TestDataDirectory, TestFileName));
        var enc1251 = CodePagesEncodingProvider.Instance.GetEncoding(CodePage);

        return File.ReadAllText(path, enc1251 ?? throw new InvalidOperationException());
    }

    private static string GetPath()
    {
        return Path.GetFullPath(Path.Combine(TestDataDirectory, TestFileName));
    }

    [Fact]
    public void Test_parse_all_async()
    {
        // Arrange
        var path = GetPath();

        // Act
        var result = CsvFileParser.ParseAsync(path);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_is_data_valid()
    {
        // Arrange
        var type = typeof(CsvFileParser);
        var method = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .FirstOrDefault(x => x.Name == "IsValidData");
        var data = GetData();

        // Act
        var result = method.Invoke(null, new[] {data});

        // Assert
        Assert.True((bool) (result ?? throw new InvalidOperationException()));
    }

    [Fact]
    public async Task Test_split_into_sections()
    {
        // Arrange
        var type = typeof(CsvFileParser);
        var method = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .FirstOrDefault(x => x.Name == "SplitIntoSections");
        var data = GetData();

        // Act
        // SplitIntoSections is asynchronous method, so it returns Task
        var task = method.Invoke(null, new[] {data}) as Task<CsvFileParser.Sections>;
        var sections = await task;

        // Assert
        // Get all public properties, if some property is null then the test is not passed
        var result = typeof(Section).GetProperties(BindingFlags.Public)
            .All(propertyInfo => propertyInfo.GetValue(sections) != null);

        Assert.True(result);
    }
}