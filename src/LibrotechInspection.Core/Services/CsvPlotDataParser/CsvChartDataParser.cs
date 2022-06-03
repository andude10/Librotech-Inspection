using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Models;
using LibrotechInspection.Core.Services.CsvPlotDataParser.Mappers;

namespace LibrotechInspection.Core.Services.CsvPlotDataParser;

public class CsvPlotDataParser : IPlotDataParser
{
    /// <summary>
    ///     Separator is the separator used in the chart data table
    /// </summary>
    private const string Separator = ";";

    /// <summary>
    ///     ParseTemperatureAsync parses data from a file into
    ///     IAsyncEnumerable ChartPoints where ChartPoint's
    ///     <code>X</code> - date, <code>Y</code> - temperature value.
    /// </summary>
    /// <param name="data">Chart value data</param>
    /// <returns>Parsed list of temperature, or an empty if data does not include temperature</returns>
    public async IAsyncEnumerable<PlotPoint> ParseTemperatureAsync(string data)
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            MissingFieldFound = null,
            Delimiter = Separator
        };

        using var reader = new StringReader(data);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<TemperatureMapper>();

        // If there is no header, then there is no data, and we return an empty value.
        try
        {
            await csv.ReadAsync();
            csv.GetRecord<PlotPoint>();
        }
        catch (HeaderValidationException)
        {
            yield break;
        }

        while (await csv.ReadAsync()) yield return csv.GetRecord<PlotPoint>();
    }

    /// <summary>
    ///     ParseHumidityAsync parses data from a file into
    ///     IAsyncEnumerable ChartPoints where ChartPoint's
    ///     <code>X</code> - date, <code>Y</code> - humidity value.
    /// </summary>
    /// <param name="data">Chart value data</param>
    /// <returns>Parsed list of humidity, or an empty if data does not include humidity</returns>
    public async IAsyncEnumerable<PlotPoint> ParseHumidityAsync(string data)
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            MissingFieldFound = null,
            Delimiter = Separator
        };

        using var reader = new StringReader(data);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<HumidityMapper>();

        // If there is no header, then there is no data, and we return an empty value.
        try
        {
            await csv.ReadAsync();
            csv.GetRecord<PlotPoint>();
        }
        catch (HeaderValidationException)
        {
            yield break;
        }

        while (await csv.ReadAsync()) yield return csv.GetRecord<PlotPoint>();
    }

    /// <summary>
    ///     ParsePressureAsync parses data from a file into
    ///     IAsyncEnumerable ChartPoints where ChartPoint's
    ///     <code>X</code> - date, <code>Y</code> - pressure value.
    /// </summary>
    /// <param name="data">Chart value data</param>
    /// <returns>Parsed list of pressure, or an empty if data does not include pressure</returns>
    public async IAsyncEnumerable<PlotPoint> ParsePressureAsync(string data)
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            MissingFieldFound = null,
            Delimiter = Separator
        };

        using var reader = new StringReader(data);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<PressureMapper>();

        // If there is no header, then there is no data, and we return an empty value.
        try
        {
            await csv.ReadAsync();
            csv.GetRecord<PlotPoint>();
        }
        catch (HeaderValidationException)
        {
            yield break;
        }

        while (await csv.ReadAsync()) yield return csv.GetRecord<PlotPoint>();
    }
}