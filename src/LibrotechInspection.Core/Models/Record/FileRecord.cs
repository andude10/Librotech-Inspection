namespace LibrotechInspection.Core.Models.Record;

public class FileRecord : Record
{
    public override string RecordType { get; } = nameof(FileRecord);
    public string FileName { get; set; }
}