using LibrotechInspection.Core.Models.Record;

namespace LibrotechInspection.Core.Interfaces;

public interface IFileRecordParser
{
    /// <summary>
    ///     Parse text-formatted record into a FileRecord
    /// </summary>
    /// <param name="path">Path file to parse</param>
    /// <returns>Parsed file, or null if something went wrong</returns>
    public Task<FileRecord?> ParseAsync(string path);
}