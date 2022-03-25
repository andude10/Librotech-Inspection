﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Librotech_Inspection.Models;
using Librotech_Inspection.Utilities.Parsers.AllDataParsers.CsvFile;
using Xunit;

namespace UnitTests.ParsersTests.AllDataParsersTests;

public class CsvFileParserTests
{
    private const string TestFileName = @"testFile.csv";
    private const string TestDataSectionsName = @"TestDataSections.json";
    private const string TestDataDirectory = @"TestData";
    private const int CodePage = 1251;

    private static CsvFileParser.Sections GetDataSections()
    {
        var path = Path.GetFullPath(Path.Combine(TestDataDirectory, TestDataSectionsName));

        return JsonSerializer.Deserialize<CsvFileParser.Sections>(File.ReadAllText(path))
               ?? throw new InvalidOperationException();
    }

    private static string GetTextData()
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
        var data = CsvFileParser.ParseAsync(path);

        // Assert
        Assert.NotNull(data);
    }

    [Fact]
    public void Test_is_data_valid()
    {
        // Arrange
        var type = typeof(CsvFileParser);
        var method = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .FirstOrDefault(x => x.Name == "IsValidData");
        var textData = GetTextData();

        // Act
        var result = method.Invoke(null, new[] {textData});

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
        var textData = GetTextData();

        // Act
        // SplitIntoSections is asynchronous method, so it returns Task
        var task = method.Invoke(null, new[] {textData})
            as Task<CsvFileParser.Sections>;
        var sections = await task;

        // Assert
        // Get all public properties, if some property is null then the test is not passed
        var result = typeof(CsvFileParser.Sections).GetProperties(BindingFlags.Public)
            .All(propertyInfo => propertyInfo.GetValue(sections) != null);

        Assert.True(result);
    }

    [Fact]
    public async Task Test_parse_sections()
    {
        // Arrange
        var type = typeof(CsvFileParser);
        var method = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .FirstOrDefault(x => x.Name == "ParseSectionsAsync");
        var sections = GetDataSections();

        // Act
        // SplitIntoSections is asynchronous method, so it returns Task
        var task = method.Invoke(null, new[] {sections})
            as Task<Data>;
        var data = await task;

        // Assert
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Test_parse_device_specification_section()
    {
        // Arrange
        var type = typeof(CsvFileParser);
        var method = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .FirstOrDefault(x => x.Name == "ParseDeviceSpecificationsSection");
        var sections = GetDataSections();

        // Act
        // SplitIntoSections is asynchronous method, so it returns Task
        var task = method.Invoke(null, new[] {sections.DeviceSpecifications})
            as Task<List<DeviceSpecification>>;
        var deviceSpecifications = await task;

        // Assert
        Assert.NotNull(deviceSpecifications);
    }

    [Fact]
    public async Task Test_parse_emergency_event_settings_and_results()
    {
        // Arrange
        var type = typeof(CsvFileParser);
        var method = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .FirstOrDefault(x => x.Name == "ParseEmergencyEventSettingsAndResults");
        var sections = GetDataSections();

        // Act
        // SplitIntoSections is asynchronous method, so it returns Task
        var task = method.Invoke(null, new[] {sections.EmergencyEventSettingsAndResults})
            as Task<List<EmergencyEventsSettings>>;
        var emergencyEventSettingsAndResults = await task;

        // Assert
        Assert.NotNull(emergencyEventSettingsAndResults);
    }

    [Fact]
    public async Task Test_parse_time_stamps()
    {
        // Arrange
        var type = typeof(CsvFileParser);
        var method = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .FirstOrDefault(x => x.Name == "ParseTimeStamps");
        var sections = GetDataSections();

        // Act
        // SplitIntoSections is asynchronous method, so it returns Task
        var task = method.Invoke(null, new[] {sections.TimeStamps})
            as Task<List<Stamp>>;
        var timeStamps = await task;

        // Assert
        Assert.NotNull(timeStamps);
    }
}