using System;
using System.Linq;
using Librotech_Inspection.Models;
using Librotech_Inspection.Utilities.DataDecorators.Representatives;

namespace Librotech_Inspection.Utilities.DataDecorators;

/// <summary>
///     The ShortSummaryDecorator is responsible for creating a short description
///     of the file - start of recording, end of recording, number of stamps, etc.
/// </summary>
public static class ShortSummaryDecorator
{
    // TODO: This is hard-coded, this is a temporary solution
    public static ShortSummary GenerateShortSummary(FileData? fileData)
    {
        if (fileData == null) throw new NullReferenceException();

        var shortSummary = new ShortSummary();

        shortSummary.SessionId =
            $"Номер сессии: {fileData.DeviceSpecifications.First(s => s.Name == "Номер сессии").Value}";
        shortSummary.SessionStart =
            $"Начало сессии: {fileData.DeviceSpecifications.First(s => s.Name == "Начало сессии").Value}";
        shortSummary.SessionEnd =
            $"Конец сессии: {fileData.DeviceSpecifications.First(s => s.Name == "Конец сессии").Value}";
        shortSummary.TotalDuration =
            $"Общая длительность: {fileData.DeviceSpecifications.First(s => s.Name == "Общая длительность").Value}";

        return shortSummary;
    }
}