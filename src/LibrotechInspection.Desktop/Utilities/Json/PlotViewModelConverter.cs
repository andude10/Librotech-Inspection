using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LibrotechInspection.Desktop.ViewModels.PlotViewModels;

namespace LibrotechInspection.Desktop.Utilities.Json;

public class PlotViewModelConverter : JsonConverter<PlotViewModel>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(PlotViewModel).IsAssignableFrom(typeToConvert);
    }

    public override PlotViewModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);

        return jsonDoc.RootElement.GetProperty(nameof(PlotViewModel.PlotType)).GetString() switch
        {
            nameof(LinePlotViewModel) => jsonDoc.RootElement.Deserialize<LinePlotViewModel>(options),
            _ => throw new JsonException("'PlotType' doesn't match a known derived type")
        } ?? throw new InvalidOperationException();
    }

    public override void Write(Utf8JsonWriter writer, PlotViewModel plotViewModel, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object) plotViewModel, options);
    }
}