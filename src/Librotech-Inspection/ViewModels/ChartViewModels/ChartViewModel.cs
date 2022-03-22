using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Wpf;
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
    
    /// <summary>
    ///     PlotModel is the model for the chart on which the PlotView renders data.
    /// </summary>
    public abstract PlotModel? PlotModel { get; set; }

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
    
    /// <summary>
    ///     BuildAsync() builds chartData for a PlotModel instance, creates a PlotModel instance.
    /// </summary>
    /// <param name="chartData">Data in text format</param>
    /// <returns></returns>
    public abstract Task BuildAsync(string chartData);
    
    /// <summary>
    ///     CreateModel() creates a new PlotModel instance based on the processed data
    /// </summary>
    /// <returns></returns>
    public abstract void CreateModel();
}