using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Librotech_Inspection.Models;
using Librotech_Inspection.Utilities.Parsers.ChartDataParsers.Mappers;

namespace Librotech_Inspection.Utilities.Parsers.ChartDataParsers;

public static class LineChartParser
{
    private const string Separator = ";";

    public static async IAsyncEnumerable<ChartPoint> ParseTemperatureAsync(string data)
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            MissingFieldFound = null,
            Delimiter = Separator
        };

        using var reader = new StringReader(data);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<TemperatureMapper>();

        try
        {
            await csv.ReadAsync();
            csv.GetRecord<ChartPoint>();
        }
        catch (HeaderValidationException)
        {
            yield break;
        }

        while (await csv.ReadAsync()) yield return csv.GetRecord<ChartPoint>();
    }

    public static async IAsyncEnumerable<ChartPoint> ParseHumidityAsync(string data)
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            MissingFieldFound = null,
            Delimiter = Separator
        };

        using var reader = new StringReader(data);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<HumidityMapper>();

        try
        {
            await csv.ReadAsync();
            csv.GetRecord<ChartPoint>();
        }
        catch (HeaderValidationException)
        {
            yield break;
        }

        while (await csv.ReadAsync()) yield return csv.GetRecord<ChartPoint>();
    }

    public static async IAsyncEnumerable<ChartPoint> ParsePressureAsync(string data)
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            MissingFieldFound = null,
            Delimiter = Separator
        };

        using var reader = new StringReader(data);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<PressureMapper>();

        try
        {
            await csv.ReadAsync();
            csv.GetRecord<ChartPoint>();
        }
        catch (HeaderValidationException)
        {
            yield break;
        }

        while (await csv.ReadAsync()) yield return csv.GetRecord<ChartPoint>();
    }
}