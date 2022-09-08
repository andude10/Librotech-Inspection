using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using DynamicData;
using LibrotechInspection.Desktop.Utilities.Json;
using OxyPlot;

namespace LibrotechInspection.Desktop.Models;

public class PlotDataContainer
{
    public PlotDataContainer()
    {
        MarkedTemperaturePoints = new SourceList<MarkedDataPoint>();
        MarkedHumidityPoints = new SourceList<MarkedDataPoint>();
        MarkedPressurePoints = new SourceList<MarkedDataPoint>();
        HumidityPoints = new List<DataPoint>();
        PressurePoints = new List<DataPoint>();
        TemperaturePoints = new List<DataPoint>();
    }

    [JsonConstructor]
    public PlotDataContainer(List<SerializableDataPoint>? serializableTemperaturePoints,
        List<SerializableDataPoint>? serializableHumidityPoints,
        List<SerializableDataPoint>? serializablePressurePoints,
        IEnumerable<MarkedDataPoint>? serializableMarkedTemperaturePoints,
        IEnumerable<MarkedDataPoint>? serializableMarkedHumidityPoints,
        IEnumerable<MarkedDataPoint>? serializableMarkedPressurePoints) : this()
    {
        TemperaturePoints = serializableTemperaturePoints?.Select(point => new DataPoint(point.X, point.Y)).ToList()
                            ?? new List<DataPoint>();
        HumidityPoints = serializableHumidityPoints?.Select(point => new DataPoint(point.X, point.Y)).ToList()
                         ?? new List<DataPoint>();
        PressurePoints = serializablePressurePoints?.Select(point => new DataPoint(point.X, point.Y)).ToList()
                         ?? new List<DataPoint>();

        if (serializableMarkedTemperaturePoints is not null)
            MarkedTemperaturePoints.AddRange(serializableMarkedTemperaturePoints);
        if (serializableMarkedHumidityPoints is not null)
            MarkedHumidityPoints.AddRange(serializableMarkedHumidityPoints);
        if (serializableMarkedPressurePoints is not null)
            MarkedPressurePoints.AddRange(serializableMarkedPressurePoints);
    }

#region Properties

    [JsonIgnore] public List<DataPoint> TemperaturePoints { get; }
    [JsonIgnore] public List<DataPoint> HumidityPoints { get; }
    [JsonIgnore] public List<DataPoint> PressurePoints { get; }

    [JsonIgnore] public SourceList<MarkedDataPoint> MarkedTemperaturePoints { get; }
    [JsonIgnore] public SourceList<MarkedDataPoint> MarkedHumidityPoints { get; }
    [JsonIgnore] public SourceList<MarkedDataPoint> MarkedPressurePoints { get; }

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

    [JsonInclude]
    public IEnumerable<MarkedDataPoint> SerializableMarkedTemperaturePoints => MarkedTemperaturePoints.Items;

    [JsonInclude] public IEnumerable<MarkedDataPoint> SerializableMarkedHumidityPoints => MarkedHumidityPoints.Items;
    [JsonInclude] public IEnumerable<MarkedDataPoint> SerializableMarkedPressurePoints => MarkedPressurePoints.Items;

#endregion
}