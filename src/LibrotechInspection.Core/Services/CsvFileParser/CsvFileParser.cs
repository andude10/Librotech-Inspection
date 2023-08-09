using System.Globalization;
using System.Security;
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
/// TODO: Everything is too hard-coded here and I also copied the code from the old version, this needs to be rewritten in the future
/// <summary>
///     CsvFileParser is for parsing csv file.
/// </summary>
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

    private readonly string[] _supportedDevicesNames = { "SX100" };

    /// <summary>
    ///     ParseAsync parses text-formatted data into a <code>Data</code> object
    ///     into a <code>Data</code> object
    /// </summary>
    /// <param name="path">Path file to parse</param>
    /// <returns>ParserResult, or null if file is incorrect or corrupted</returns>
    /// TODO: Split ParseAsync into methods
    public async Task<ParserResult> ParseAsync(string path)
    {
        var openFileResult = await OpenFile(path);

        if (openFileResult.ReadAllTextException is not null) return HandleReadAllTextException(openFileResult);

        if (openFileResult.DeviceSupported is false) Logger.Error("Device that recorded the file is not supported");
        if (openFileResult.IsValidData is false) Logger.Error("An incorrect or corrupted file was selected");

        if (openFileResult.ExtractedText is null)
            return new ParserResult(IncorrectOrCorruptedFile: openFileResult.IsValidData,
                DeviceSupported: openFileResult.DeviceSupported);

        try
        {
            var sections = SplitIntoSections(openFileResult.ExtractedText);
            var record = await ParseSectionsAsync(sections);

            return new ParserResult(record,
                DeviceSupported: openFileResult.DeviceSupported,
                IncorrectOrCorruptedFile: openFileResult.IsValidData);
        }
        catch (Exception exception)
        {
            var parserResult = new ParserResult(DeviceSupported: openFileResult.DeviceSupported,
                IncorrectOrCorruptedFile: openFileResult.IsValidData,
                ParserException: exception);
            Logger.Error($"Exception occurred while parsing csv file. Parsing result: \n" +
                         $" {nameof(parserResult.IncorrectOrCorruptedFile)} is {parserResult.IncorrectOrCorruptedFile} \n" +
                         $" {nameof(parserResult.DeviceSupported)} is {parserResult.DeviceSupported}. \n" +
                         $" Exception: {exception.Message + exception.StackTrace}");
            return parserResult;
        }
    }

    private static ParserResult HandleReadAllTextException(OpenFileResult openFileResult)
    {
        if (openFileResult.ReadAllTextException is PathTooLongException)
            return new ParserResult(ParserException: openFileResult.ReadAllTextException, PathTooLong: true);

        if (openFileResult.ReadAllTextException is UnauthorizedAccessException |
            openFileResult.ReadAllTextException is SecurityException)
            return new ParserResult(ParserException: openFileResult.ReadAllTextException, UnauthorizedAccess: true);

        if (openFileResult.ReadAllTextException is FileNotFoundException)
            return new ParserResult(ParserException: openFileResult.ReadAllTextException, FileNotFound: true);

        return new ParserResult(ParserException: openFileResult.ReadAllTextException, CanReachFile: false);
    }

    /// <summary>
    ///     Validates the file at a given path and extracts text from it
    /// </summary>
    private async Task<OpenFileResult> OpenFile(string path)
    {
        if (!path.Contains(".csv")) return new OpenFileResult(IsValidData: false);

        var enc1251 = CodePagesEncodingProvider.Instance.GetEncoding(FileCodePage)
                      ?? throw new InvalidOperationException("Can't find enc1251 encoding");

        string text;
        try
        {
            text = await File.ReadAllTextAsync(path, enc1251);
        }
        catch (Exception exception)
        {
            Logger.Error($"Exception while reading file: {exception.Message + exception.StackTrace}");
            return new OpenFileResult(ReadAllTextException: exception);
        }

        var isValid = !string.IsNullOrEmpty(text) &&
                      text.Contains(DeviceSpecificationsSectionName) &
                      text.Contains(PlotDataSectionName);
        var isDeviceSupported = _supportedDevicesNames.Any(text.Contains);

        return new OpenFileResult(text, isValid, isDeviceSupported);
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
        var nextSectionIndex =
            new[] { emergencyEventSettingsIndex, timeStampsIndex, plotDataIndex }.First(id => id != -1);
        var deviceInfoData = data[deviceInfoIndex .. nextSectionIndex].Trim();

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

            result.DeviceAlarmSettings = emergencyEventSettingsData;
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

        var emergencyEventTask = !string.IsNullOrEmpty(sections.DeviceAlarmSettings)
            ? ParseEmergencyEventSettingsAndResults(sections.DeviceAlarmSettings)
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
            DeviceAlarmSettings = emergencyEventTask?.Result,
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
    private async Task<List<DeviceAlarmSetting>> ParseEmergencyEventSettingsAndResults(string section)
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            MissingFieldFound = null,
            Delimiter = EmergencyEventSettingsAndResultsSeparator
        };

        using var reader = new StringReader(section);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<DeviceAlarmSettingMapper>();

        return await csv.GetRecordsAsync<DeviceAlarmSetting>().ToListAsync();
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

    private record OpenFileResult(string? ExtractedText = null, bool? IsValidData = null, bool? DeviceSupported = null,
        Exception? ReadAllTextException = null);

    /// <summary>
    ///     The Sections object that represents the sections that the file has.
    ///     If the property is null, then this section is not in the file
    /// </summary>
    public class Sections
    {
        public string? DeviceSpecifications { get; set; }
        public string? DeviceAlarmSettings { get; set; }
        public string? TimeStamps { get; set; }
        public string? PlotData { get; set; }
    }
}