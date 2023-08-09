using LibrotechInspection.Core.Models.Record;

namespace LibrotechInspection.Core.Interfaces;

public record ParserResult(FileRecord? FileRecord = null,
    bool? IncorrectOrCorruptedFile = null,
    bool? DeviceSupported = null,
    bool PathTooLong = false,
    bool UnauthorizedAccess = false,
    bool FileNotFound = false,
    bool CanReachFile = true,
    Exception? ParserException = null);

public interface IFileRecordParser
{
    /// <summary>
    ///     Parse text-formatted record into a FileRecord
    /// </summary>
    /// <param name="path">Path file to parse</param>
    /// <returns>Parsed file, or null if file is incorrect or corrupted</returns>
    public Task<ParserResult> ParseAsync(string path);
}