using System;
using System.Linq;
using LibrotechInspection.Core.Models.Record;
using LibrotechInspection.Desktop.Utilities.DataDecorators.Presenters;

namespace LibrotechInspection.Desktop.Utilities.DataDecorators;

/// <summary>
///     The ShortSummaryDecorator is responsible for generating a short description
///     of the file - start of recording, end of recording, number of stamps, etc.
/// </summary>
public static class ShortSummaryDecorator
{
    // TODO: This is hard-coded, this is a temporary solution
    public static ShortSummaryPresenter GenerateShortSummary(Record? fileData)
    {
        if (fileData == null) throw new NullReferenceException();

        var shortSummary = new ShortSummaryPresenter();

        shortSummary.SessionId = fileData.DeviceSpecifications.First(s => s.Name == "Номер сессии").Value;
        shortSummary.SessionStart = fileData.DeviceSpecifications.First(s => s.Name == "Начало сессии").Value;
        shortSummary.SessionEnd = fileData.DeviceSpecifications.First(s => s.Name == "Конец сессии").Value;
        shortSummary.TotalDuration = fileData.DeviceSpecifications.First(s => s.Name == "Общая длительность").Value;

        return shortSummary;
    }
}