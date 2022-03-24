using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Librotech_Inspection.Models;
using Librotech_Inspection.Utilities.Interactions;
using Librotech_Inspection.Utilities.Parsers.AllDataParsers.CsvFile.Mappers;

namespace Librotech_Inspection.Utilities.Parsers.AllDataParsers.CsvFile;

/* PARSING ALGORITHM FOR .csv FILE
 * 1. Check if the file is damaged or(and) has the wrong format. 
 *    If it's true, return null and display an error to the user.
 * 
 * 2. Split the file into sections (in text format) that it has.
 *    For example, the "DeviceSpecificationsPreview" section. Each file 
 *    must have a section with data for the chart.
 * 
 * 3. Cast each section to the corresponding class (see Model folder).
 *
 * 4. Create a Data object and populate the resulting sections
 *    in its properties.
 */

/// <summary>
///     CsvFileParser is responsible for parsing the csv file.
/// </summary>
/// TODO: Everything is too hard-coded here and I also copied the code from the old version, this needs to be rewritten in the future
public static class CsvFileParser
{
    private const int FileCodePage = 1251;

    private const string SectionsSeparator = "***************************************";
    private const string DeviceSpecificationsSeparator = ": ;";
    private const string EmergencyEventSettingsAndResultsSeparator = ";";
    private const string TimeStampsSeparator = "---------------------------";
    private const string StampItemsSeparator = ": ;";

    /// <summary>
    ///     ParseAsync parses text-formatted data
    ///     into a <code>Data</code> object
    /// </summary>
    /// <param name="path">Path file to parse</param>
    /// <returns>Parsed file, or null if something went wrong</returns>
    public static async Task<Data?> ParseAsync(string path)
    {
        var enc1251 = CodePagesEncodingProvider.Instance.GetEncoding(FileCodePage);
        var data = await File.ReadAllTextAsync(path, enc1251 ?? throw new InvalidOperationException());

        if (!IsValidData(data))
        {
            Debug.WriteLine("During csv file parsing: An incorrect or corrupted file was selected");
            ErrorInteractions.Error.Handle("An incorrect or corrupted file was selected").Subscribe();
            return null;
        }

        Data file;

        try
        {
            file = await ParseSectionsAsync(await SplitIntoSections(data));
        }
        catch (Exception e)
        {
            Debug.WriteLine(
                "During csv file parsing: An error has occurred although the file is in the correct format");
            ErrorInteractions.Error.Handle("An internal application error has occurred").Subscribe();
            return null;
        }

        file.FileName = Path.GetFileName(path);

        return file;
    }

    /// <summary>
    ///     IsValidData checks if the text-formatted data is valid.
    /// </summary>
    /// <returns>True if valid</returns>
    /// TODO: This is implemented stupidly at this moment, it should be fixed in the future.
    /// (It just checks if there are key rows in the data)
    private static bool IsValidData(string data)
    {
        if (string.IsNullOrEmpty(data)) return false;

        return data.Contains("Информация об устройстве") &
               data.Contains("Настройки аварийных событий и результаты") &
               data.Contains("Дата/время");
    }

    /// <summary>
    ///     SplitIntoSections splits the entire text file into Sections
    /// </summary>
    /// <returns>Sections from data</returns>
    /// TODO: This is implemented stupidly at this moment, it should be fixed in the future.
    private static async Task<Sections> SplitIntoSections(string data)
    {
        var arr = new List<string>();
        await Task.Factory.StartNew(() =>
        {
            data = data.Replace("  ", string.Empty)
                .Trim();

            arr = data.Split(SectionsSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x.Trim()))
                .ToList();

            // CRUTCHES STARTS

            // The first is the name of the first section, so we remove
            arr.Remove(arr.First());
            // The last line of each item is the name of the next section, so we remove
            arr[0] = arr[0].Replace("Настройки аварийных событий и результаты", string.Empty);
            arr[1] = arr[1].Replace("Штампы времени", string.Empty);

            // CRUTCHES ENDS
        });

        // In the file, unfortunately, the stamp section and the data for 
        // the chart section are not separated, so we have to use crutches
        // CRUTCH STARTS
        var tempArr = arr[2].Split(TimeStampsSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x.Trim()))
            .ToList();

        var chartData = tempArr.Last();
        tempArr.RemoveAt(tempArr.Count - 1);
        var stamps = string.Join(TimeStampsSeparator, tempArr);
        // CRUTCH ENDS

        return new Sections
        {
            DeviceSpecifications = arr[0].Trim(),
            EmergencyEventSettingsAndResults = arr[1].Trim(),
            TimeStamps = stamps.Trim(),
            ChartData = chartData.Trim()
        };
    }

    /// <summary>
    ///     The ParseSectionsAsync creates a Data object,
    ///     then populates its properties from the <code>Sections</code>
    ///     object (parses the <code>Section</code> object)
    /// </summary>
    /// <param name="sections">Data sections</param>
    /// <returns></returns>
    private static async Task<Data> ParseSectionsAsync(Sections sections)
    {
        return new Data
        {
            DeviceSpecifications = !string.IsNullOrEmpty(sections.DeviceSpecifications)
                ? await ParseDeviceSpecificationsSection(sections.DeviceSpecifications)
                : null,
            EmergencyEventsSettings = !string.IsNullOrEmpty(sections.EmergencyEventSettingsAndResults)
                ? await ParseEmergencyEventSettingsAndResults(sections.EmergencyEventSettingsAndResults)
                : null,
            Stamps = !string.IsNullOrEmpty(sections.TimeStamps)
                ? await ParseTimeStamps(sections.TimeStamps)
                : null,
            ChartData = sections.ChartData ?? throw new InvalidOperationException()
        };
    }

    /// <summary>
    ///     The ParseDeviceSpecificationsSection parses the
    ///     "DeviceSpecificationsPreview" section of a file
    ///     from text to List of DeviceSpecification
    /// </summary>
    /// <param name="section">The "DeviceSpecificationsPreview" section from the data</param>
    /// <returns></returns>
    private static async Task<List<DeviceSpecification>> ParseDeviceSpecificationsSection(string section)
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            MissingFieldFound = null,
            HasHeaderRecord = false,
            Delimiter = DeviceSpecificationsSeparator
        };

        using var reader = new StringReader(section);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<DeviceSpecificationMapper>();

        return await csv.GetRecordsAsync<DeviceSpecification>().ToListAsync();
    }

    /// <summary>
    ///     The ParseEmergencyEventSettingsAndResults parses the
    ///     "EmergencyEventSettingsAndResults" section of a file
    ///     from text to List of EmergencyEventsSettingsPreview
    /// </summary>
    /// <param name="section">The "EmergencyEventSettingsAndResults" section from the data</param>
    /// <returns></returns>
    private static async Task<List<EmergencyEventsSettings>> ParseEmergencyEventSettingsAndResults(string section)
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            MissingFieldFound = null,
            Delimiter = EmergencyEventSettingsAndResultsSeparator
        };

        using var reader = new StringReader(section);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<EmergencyEventsSettingsMapper>();

        return await csv.GetRecordsAsync<EmergencyEventsSettings>().ToListAsync();
    }

    /// <summary>
    ///     The ParseTimeStamps parses the "TimeStamps"
    ///     section of a file from text to List of stamps
    /// </summary>
    /// <param name="section">The "TimeStamps" section from the data</param>
    /// <returns></returns>
    private static async Task<List<Stamp>> ParseTimeStamps(string section)
    {
        var stampsText = section.Split(TimeStampsSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x.Trim()))
            .ToList();

        var config = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = false,
            MissingFieldFound = null,
            Delimiter = ": ;"
        };

        var stamps = new List<Stamp>();

        for (var i = 0; i < stampsText.Count; i++)
        {
            stampsText[i] = stampsText[i].Replace($"Штамп {i + 1}", string.Empty)
                .Replace("  ", string.Empty)
                .Trim();

            using var reader = new StringReader(stampsText[i]);
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<StampItemMapper>();
            stamps.Add(new Stamp($"Штамп {i + 1}", await csv.GetRecordsAsync<StampItem>().ToListAsync()));
        }

        return stamps;
    }

    /// <summary>
    ///     The Sections object that represents the sections that the file has.
    ///     If the property is null, then this section is not in the file
    /// </summary>
    public class Sections
    {
        public string? DeviceSpecifications { get; set; }
        public string? EmergencyEventSettingsAndResults { get; set; }
        public string? TimeStamps { get; set; }
        public string? ChartData { get; set; }
    }
}