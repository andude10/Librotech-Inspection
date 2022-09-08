using System.Collections.Generic;
using OxyPlot;

namespace LibrotechInspection.Desktop.Services;

public interface IPlotModelManager
{
    public PlotModel PlotModel { get; }

    public void MarkPoint(DataPoint point, Element parentElement);

    public void AddDateTimeAxis();

    public void AddTemperature(IEnumerable<DataPoint> temperaturePoints);

    public void ShowOrHideTemperature(bool display);

    public void AddHumidity(IEnumerable<DataPoint> humidityPoints);

    public void ShowOrHideHumidity(bool display);

    public void AddPressure(IEnumerable<DataPoint> pressurePoints);

    public void ShowOrHidePressure(bool display);
}