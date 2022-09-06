using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using LibrotechInspection.Desktop.Utilities.Json;
using OxyPlot;

namespace LibrotechInspection.Desktop.Models;

public class LinePlotData
{
    public LinePlotData()
    {
        HumidityPoints = new List<DataPoint>();
        PressurePoints = new List<DataPoint>();
        TemperaturePoints = new List<DataPoint>();
    }

    [JsonConstructor]
    public LinePlotData(List<SerializableDataPoint> serializableTemperaturePoints,
        List<SerializableDataPoint> serializableHumidityPoints,
        List<SerializableDataPoint> serializablePressurePoints)
    {
        TemperaturePoints = serializableTemperaturePoints.Select(point => new DataPoint(point.X, point.Y)).ToList();
        HumidityPoints = serializableHumidityPoints.Select(point => new DataPoint(point.X, point.Y)).ToList();
        PressurePoints = serializablePressurePoints.Select(point => new DataPoint(point.X, point.Y)).ToList();
    }

    [JsonIgnore] public List<DataPoint> TemperaturePoints { get; }

    [JsonIgnore] public List<DataPoint> HumidityPoints { get; }

    [JsonIgnore] public List<DataPoint> PressurePoints { get; }

    [JsonInclude]
    public List<SerializableDataPoint> SerializableTemperaturePoints =>
        TemperaturePoints.Select(point => new SerializableDataPoint(point.X, point.Y)).ToList();

    [JsonInclude]
    public List<SerializableDataPoint> SerializableHumidityPoints =>
        HumidityPoints.Select(point => new SerializableDataPoint(point.X, point.Y)).ToList();

    [JsonInclude]
    public List<SerializableDataPoint> SerializablePressurePoints =>
        PressurePoints.Select(point => new SerializableDataPoint(point.X, point.Y)).ToList();
}