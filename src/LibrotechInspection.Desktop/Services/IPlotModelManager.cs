using System;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Series;

namespace LibrotechInspection.Desktop.Services;

public interface IPlotModelManager
{
    public PlotModel PlotModel { get; }

    public void MarkPoint(DataPoint point, Series parentSeries);

    public void AddDateTimeAxis();

    public void AddTemperature(IEnumerable<DataPoint> temperaturePoints);

    public void ShowOrHideTemperature(bool display);

    public void AddHumidity(IEnumerable<DataPoint> humidityPoints);

    public void ShowOrHideHumidity(bool display);

    public void AddPressure(IEnumerable<DataPoint> pressurePoints);

    public void ShowOrHidePressure(bool display);
}