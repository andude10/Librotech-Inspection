using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using LibrotechInspection.Desktop.Utilities.Json;
using OxyPlot;

namespace LibrotechInspection.Desktop.Models;

public class PlotDataContainer
{
    public PlotDataContainer()
    {
        MarkedTemperaturePoints = new List<MarkedDataPoint>();
        MarkedHumidityPoints = new List<MarkedDataPoint>();
        MarkedPressurePoints = new List<MarkedDataPoint>();
        HumidityPoints = new List<DataPoint>();
        PressurePoints = new List<DataPoint>();
        TemperaturePoints = new List<DataPoint>();
    }

    [JsonConstructor]
    public PlotDataContainer(List<MarkedDataPoint>? markedTemperaturePoints,
        List<MarkedDataPoint>? markedHumidityPoints,
        List<MarkedDataPoint>? markedPressurePoints,
        List<SerializableDataPoint>? serializableTemperaturePoints,
        List<SerializableDataPoint>? serializableHumidityPoints,
        List<SerializableDataPoint>? serializablePressurePoints) : this()
    {
        if (serializableTemperaturePoints is not null)
            TemperaturePoints = serializableTemperaturePoints.Select(point => new DataPoint(point.X, point.Y))
                .ToList();
        if (serializableHumidityPoints is not null)
            HumidityPoints = serializableHumidityPoints.Select(point => new DataPoint(point.X, point.Y))
                .ToList();
        if (serializablePressurePoints is not null)
            PressurePoints = serializablePressurePoints.Select(point => new DataPoint(point.X, point.Y))
                .ToList();
        if (markedTemperaturePoints is not null) MarkedTemperaturePoints = markedTemperaturePoints;
        if (markedHumidityPoints is not null) MarkedHumidityPoints = markedHumidityPoints;
        if (markedPressurePoints is not null) MarkedPressurePoints = markedPressurePoints;
    }

#region Properties

    [JsonIgnore] public List<DataPoint> TemperaturePoints { get; }
    [JsonIgnore] public List<DataPoint> HumidityPoints { get; }
    [JsonIgnore] public List<DataPoint> PressurePoints { get; }

    [JsonInclude] public List<MarkedDataPoint> MarkedTemperaturePoints { get; }
    [JsonInclude] public List<MarkedDataPoint> MarkedHumidityPoints { get; }
    [JsonInclude] public List<MarkedDataPoint> MarkedPressurePoints { get; }

#endregion

#region Properties for serialization

    [JsonInclude]
    public List<SerializableDataPoint> SerializableTemperaturePoints =>
        TemperaturePoints.Select(point => new SerializableDataPoint(point.X, point.Y)).ToList();

    [JsonInclude]
    public List<SerializableDataPoint> SerializableHumidityPoints =>
        HumidityPoints.Select(point => new SerializableDataPoint(point.X, point.Y)).ToList();

    [JsonInclude]
    public List<SerializableDataPoint> SerializablePressurePoints =>
        PressurePoints.Select(point => new SerializableDataPoint(point.X, point.Y)).ToList();

#endregion
}