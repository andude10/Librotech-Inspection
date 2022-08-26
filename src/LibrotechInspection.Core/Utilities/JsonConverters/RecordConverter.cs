using System.Text.Json;
using System.Text.Json.Serialization;
using LibrotechInspection.Core.Models.Record;

namespace LibrotechInspection.Core.Utilities.JsonConverters;

public class RecordConverter : JsonConverter<Record>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(Record).IsAssignableFrom(typeToConvert);
    }

    public override Record Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);

        return jsonDoc.RootElement.GetProperty(nameof(Record.RecordType)).GetString() switch
        {
            nameof(FileRecord) => jsonDoc.RootElement.Deserialize<FileRecord>(options),
            _ => throw new JsonException("'RecordType' doesn't match a known derived type")
        } ?? throw new InvalidOperationException();
    }

    public override void Write(Utf8JsonWriter writer, Record plotViewModel, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object) plotViewModel, options);
    }
}