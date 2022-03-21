using System.Threading.Tasks;
using OxyPlot;
using ReactiveUI;

namespace Librotech_Inspection.ViewModels.ChartViewModels;

/// <summary>
///     ChartViewModel represents the chart View and is
///     responsible for building the chart and responding to chart events
/// </summary>
public abstract class ChartViewModel : ReactiveObject
{
    private bool _showHumidity = true;

    private bool _showPressure = true;

    private bool _showTemperature = true;
    public abstract PlotModel PlotModel { get; set; }

    public bool ShowTemperature
    {
        get => _showTemperature;
        set => this.RaiseAndSetIfChanged(ref _showTemperature, value);
    }

    public bool ShowHumidity
    {
        get => _showHumidity;
        set => this.RaiseAndSetIfChanged(ref _showHumidity, value);
    }

    public bool ShowPressure
    {
        get => _showPressure;
        set => this.RaiseAndSetIfChanged(ref _showPressure, value);
    }

    public abstract Task BuildAsync(string fileData);
}