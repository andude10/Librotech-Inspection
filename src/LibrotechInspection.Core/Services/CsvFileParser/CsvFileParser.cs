using System.Diagnostics;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Models;
using LibrotechInspection.Core.Models.Record;
using LibrotechInspection.Core.Services.CsvFileParser.Mappers;

namespace LibrotechInspection.Core.Services.CsvFileParser;


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
public class CsvFileParser : IFileRecordParser
{
    private const int FileCodePage = 1251;

    private const string SectionsSeparator = "***************************************";
    private const string DeviceSpecificationsSeparator = ": ;";
    private const string EmergencyEventSettingsAndResultsSeparator = ";";
    private const string TimeStampsSeparator = "---------------------------";
    private const string StampItemsSeparator = ": ;";
    private const string EmergencyEventsAndChartDataSeparator
        = "/r/n/r/n";

    /// <summary>
    ///     ParseAsync parses text-formatted data
    ///     into a <code>Data</code> object
    /// </summary>;
    /// <param name="path">Path file to parse</param>
    /// <returns>Parsed file, or null if something went wrong</returns>
    public async Task<FileRecord?> ParseAsync(string path)
    {
        var enc1251 = CodePagesEncodingProvider.Instance.GetEncoding(FileCodePage);
        var data = await File.ReadAllTextAsync(path, enc1251 ?? throw new InvalidOperationException());

        if (!IsValidData(data))
        {
            Debug.WriteLine("During csv file parsing: An incorrect or corrupted file was selected");
            return null;
        }

        FileRecord file;

        try
        {
            var sections = await SplitIntoSections(data);
            file = await ParseSectionsAsync(sections);
        }
        catch (Exception e)
        {
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
    private bool IsValidData(string data)
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
    private async Task<Sections> SplitIntoSections(string data)
    {
        var arr = new List<string>();
        var hasStamps = false;
        var hasEmergencyEvent = false;
        await Task.Factory.StartNew(() =>
        {
            data = data.Replace("  ", string.Empty)
                .Trim();

            arr = data.Split(SectionsSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x.Trim()))
                .ToList();

            // The first is the name of the first section, so we remove
            arr.Remove(arr.First());

            hasEmergencyEvent = arr[0].Contains("Настройки аварийных событий и результаты");
            if (hasEmergencyEvent)
            {
                // The last line of each item is the name of the next section, so we remove
                arr[0] = arr[0].Replace("Настройки аварийных событий и результаты", string.Empty);   
            }
            
            hasStamps = arr[1].Contains("Штампы времени");
            if (hasStamps)
            {
                // The last line of each item is the name of the next section, so we remove
                arr[1] = arr[1].Replace("Штампы времени", string.Empty);
            }
        });
        
        var result = new Sections();
        
        // The file structure does not have explicit delimiters, so I do all of this
        // I don't want to comment on this...
        if (hasStamps)
        {
            var temp = arr[2].Split(TimeStampsSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x.Trim()))
                .ToList();

            var chartData = temp.Last(); 
            temp.RemoveAt(temp.Count - 1);
            var stamps = string.Join(TimeStampsSeparator, temp);

            result.TimeStamps = stamps.Trim();
            result.ChartData = chartData.Trim();
            result.EmergencyEventSettingsAndResults = arr[1].Trim();
        }
        else
        {
            if (hasEmergencyEvent)
            {
                var nextString = new StringBuilder();
                var temp1 = arr.Last().Split("\r\n").ToList();
                var temp2 = new List<string>();
                
                while (!nextString.Equals("Дата/время;Температура"))
                {
                    temp2.Add(nextString.ToString());
                    temp1.Remove(nextString.ToString());
                    nextString.Clear();
                    nextString.Append(temp1.First());
                }

                var chartData = string.Join("\r\n", temp1);
                var emergencyEventSettingsAndResults = string.Join("\r\n", temp2);
            
                result.EmergencyEventSettingsAndResults = emergencyEventSettingsAndResults;
                result.ChartData = chartData;
            }
            else
            {
                result.ChartData = arr.Last().Trim();
            }
        }

        result.DeviceSpecifications = arr[0];

        return result;
    }

    /// <summary>
    ///     The ParseSectionsAsync creates a Data object,
    ///     then populates its properties from the <code>Sections</code>
    ///     object (parses the <code>Section</code> object)
    /// </summary>
    /// <param name="sections">Data sections</param>
    /// <returns></returns>
    private async Task<FileRecord> ParseSectionsAsync(Sections sections)
    {
        return new FileRecord
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
            PlotData = sections.ChartData ?? throw new InvalidOperationException()
        };
    }

    /// <summary>
    ///     The ParseDeviceSpecificationsSection parses the
    ///     "DeviceSpecificationsPreview" section of a file
    ///     from text to List of DeviceCharacteristic
    /// </summary>
    /// <param name="section">The "DeviceSpecificationsPreview" section from the data</param>
    /// <returns></returns>
    private async Task<List<DeviceCharacteristic>> ParseDeviceSpecificationsSection(string section)
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

        return await csv.GetRecordsAsync<DeviceCharacteristic>().ToListAsync();
    }

    /// <summary>
    ///     The ParseEmergencyEventSettingsAndResults parses the
    ///     "EmergencyEventSettingsAndResults" section of a file
    ///     from text to List of EmergencyEventsSettingsPreview
    /// </summary>
    /// <param name="section">The "EmergencyEventSettingsAndResults" section from the data</param>
    /// <returns></returns>
    private async Task<List<EmergencyEventsSettings>> ParseEmergencyEventSettingsAndResults(string section)
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
    private async Task<List<Stamp>> ParseTimeStamps(string section)
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