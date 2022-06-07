using LibrotechInspection.Core.Models;

namespace LibrotechInspection.Core.Interfaces;

public interface IPlotDataParser
{
    public IAsyncEnumerable<PlotPoint> ParseTemperatureAsync(string data);
    public IAsyncEnumerable<PlotPoint> ParseHumidityAsync(string data);
    public IAsyncEnumerable<PlotPoint> ParsePressureAsync(string data);
}