using System.Diagnostics;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Models;
using LibrotechInspection.Core.Models.Record;
using LibrotechInspection.Core.Services.CsvFileParser.Mappers;
using NLog;

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

    private const string DeviceSpecificationsSeparator = ": ;";
    private const string EmergencyEventSettingsAndResultsSeparator = ";";
    private const string TimeStampsSeparator = "---------------------------";

    private const string SectionSeparator = "***************************************";
    private const string DeviceSpecificationsSectionName = "Информация об устройстве";
    private const string EmergencyEventSettingsSectionName = "Настройки аварийных событий и результаты";
    private const string PlotDataSectionName = "Дата/время";
    private const string TimeStampsSectionName = "Штампы времени";
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    ///     ParseAsync parses text-formatted data
    ///     into a <code>Data</code> object
    /// </summary>
    /// <param name="path">Path file to parse</param>
    /// <returns>Parsed file, or null if file is incorrect or corrupted</returns>
    public async Task<FileRecord?> ParseAsync(string path)
    {
        var data = await ExtractData(path);

        if (data == null)
        {
            Logger.Error("During csv file parsing: An incorrect or corrupted file was selected");
            return null;
        }

        var sections = SplitIntoSections(data);
        var file = await ParseSectionsAsync(sections);

        file.FileName = Path.GetFileName(path);

        return file;
    }

    /// <summary>
    ///     ExtractData extract data from file
    /// </summary>
    /// <returns>Data, or null if data is invalid</returns>
    private async Task<string?> ExtractData(string path)
    {
        if (!path.Contains(".csv")) return null;

        var enc1251 = CodePagesEncodingProvider.Instance.GetEncoding(FileCodePage);
        var data = await File.ReadAllTextAsync(path, enc1251 ?? throw new InvalidOperationException());

        var isValid = !string.IsNullOrEmpty(data) &&
                      data.Contains("Информация об устройстве") &
                      data.Contains("Дата/время");

        return isValid ? data : null;
    }

    /// <summary>
    ///     Splits the entire text file into Sections
    /// </summary>
    /// <returns>Sections from data</returns>
    private Sections SplitIntoSections(string data)
    {
        data = data.Replace(SectionSeparator, string.Empty)
            .Replace("\n\n", string.Empty)
            .Replace("\t", string.Empty);

        // find the index where the section is located in the file
        var deviceInfoIndex = data.IndexOf(DeviceSpecificationsSectionName, StringComparison.Ordinal);
        var emergencyEventSettingsIndex = data.IndexOf(EmergencyEventSettingsSectionName, StringComparison.Ordinal);
        var timeStampsIndex = data.IndexOf(TimeStampsSectionName, StringComparison.Ordinal);
        var plotDataIndex = data.IndexOf(PlotDataSectionName, StringComparison.Ordinal);

        var result = new Sections();

        // data is between the current section name, and the next section name
        var deviceInfoData = data[deviceInfoIndex .. emergencyEventSettingsIndex].Trim();

        // we get only the data, without the name of the section
        // also there must be no spaces at the beginning of the line
        deviceInfoData = string.Concat(
            deviceInfoData.Replace(DeviceSpecificationsSectionName, string.Empty)
                .SkipWhile(char.IsWhiteSpace)); // remove spaces at the beginning, if any

        result.DeviceSpecifications = deviceInfoData;

        if (emergencyEventSettingsIndex != -1)
        {
            var emergencyEventSettingsData = timeStampsIndex == -1
                ? data[emergencyEventSettingsIndex .. plotDataIndex].Trim()
                : data[emergencyEventSettingsIndex .. timeStampsIndex].Trim();

            emergencyEventSettingsData = string.Concat(
                emergencyEventSettingsData.Replace(EmergencyEventSettingsSectionName, string.Empty)
                    .SkipWhile(char.IsWhiteSpace)); // remove spaces at the beginning, if any

            result.EmergencyEventSettings = emergencyEventSettingsData;
        }

        if (timeStampsIndex != -1)
        {
            var timeStampsData = data[timeStampsIndex .. plotDataIndex].Trim();
            timeStampsData = string.Concat(timeStampsData.Replace(TimeStampsSectionName, string.Empty)
                .SkipWhile(char.IsWhiteSpace)); // remove spaces at the beginning, if any

            result.TimeStamps = timeStampsData;
        }

        result.PlotData = data[plotDataIndex ..];

        return result;
    }

    /// <summary>
    ///     Creates a Data object, then populates its properties from
    ///     the Sections object (parses the Section object)
    /// </summary>
    /// <param name="sections">Data sections</param>
    /// <returns></returns>
    private async Task<FileRecord> ParseSectionsAsync(Sections sections)
    {
        var deviceSpecificationsTask = !string.IsNullOrEmpty(sections.DeviceSpecifications)
            ? ParseDeviceSpecificationsSection(sections.DeviceSpecifications)
            : null;

        var emergencyEventTask = !string.IsNullOrEmpty(sections.EmergencyEventSettings)
            ? ParseEmergencyEventSettingsAndResults(sections.EmergencyEventSettings)
            : null;

        var timeStampsTask = !string.IsNullOrEmpty(sections.TimeStamps)
            ? ParseTimeStamps(sections.TimeStamps)
            : null;

        var parseTasks = new List<Task>();

        if (deviceSpecificationsTask != null) parseTasks.Add(deviceSpecificationsTask);
        if (emergencyEventTask != null) parseTasks.Add(emergencyEventTask);
        if (timeStampsTask != null) parseTasks.Add(timeStampsTask);

        await Task.WhenAll(parseTasks);

        return new FileRecord
        {
            DeviceSpecifications = deviceSpecificationsTask?.Result,
            EmergencyEventsSettings = emergencyEventTask?.Result,
            Stamps = timeStampsTask?.Result,
            PlotData = sections.PlotData ?? throw new InvalidOperationException()
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
        public string? EmergencyEventSettings { get; set; }
        public string? TimeStamps { get; set; }
        public string? PlotData { get; set; }
    }
}