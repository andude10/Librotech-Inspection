using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LibrotechInspection.Desktop.ViewModels.PlotViewModels;

namespace LibrotechInspection.Desktop.Utilities.Json;

public class LinePlotViewModelBaseConverter : JsonConverter<LinePlotViewModelBase>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(LinePlotViewModelBase).IsAssignableFrom(typeToConvert);
    }

    public override LinePlotViewModelBase Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);

        return jsonDoc.RootElement.GetProperty(nameof(LinePlotViewModelBase.PlotType)).GetString() switch
        {
            nameof(LinePlotViewModel) => jsonDoc.RootElement.Deserialize<LinePlotViewModel>(options),
            _ => throw new JsonException("'PlotType' doesn't match a known derived type")
        } ?? throw new InvalidOperationException();
    }

    public override void Write(Utf8JsonWriter writer, LinePlotViewModelBase linePlotViewModelBase,
        JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object) linePlotViewModelBase, options);
    }
}