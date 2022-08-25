using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LibrotechInspection.Core.Models.Record;

namespace LibrotechInspection.Desktop.Tests.TestData;

public static class TestDataProvider
{
    private const string CsvFileName = "testFile.csv";
    private const string TestDataFileName = @"testChartData.csv";
    private const string RecordFileName = "serializedRecord.json";
    private const string TestDataDirectory = "TestData";

    public static string GetCsvFilePath()
    {
        return Path.GetFullPath(Path.Combine(TestDataDirectory, CsvFileName));
    }

    public static string GetPlotData()
    {
        var path = Path.GetFullPath(Path.Combine(TestDataDirectory, TestDataFileName));
        return File.ReadAllText(path);
    }

    public static async Task<Record> GetRecordOne()
    {
        var json = await File.ReadAllTextAsync(Path.GetFullPath(Path.Combine(TestDataDirectory, RecordFileName)));
        return JsonSerializer.Deserialize<FileRecord>(json) ?? throw new InvalidOperationException();
    }
}