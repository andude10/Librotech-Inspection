using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LibrotechInspection.Core.Models.Record;

namespace LibrotechInspection.Desktop.Tests.TestData;

public static class TestDataProvider
{
    private const string CsvFileName = "testFile.csv";
    private const string RecordFileName = "serializedRecord.json";
    private const string TestDataDirectory = "TestData";
    
    public static string GetCsvFilePath() => Path.GetFullPath(Path.Combine(TestDataDirectory, CsvFileName));
    public static async Task<Record> GetRecord()
    {
        var json = await File.ReadAllTextAsync(Path.GetFullPath(Path.Combine(TestDataDirectory, RecordFileName)));
        return JsonSerializer.Deserialize<FileRecord>(json) ?? throw new InvalidOperationException();
    }
}