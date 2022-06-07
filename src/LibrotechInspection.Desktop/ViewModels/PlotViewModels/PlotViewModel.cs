using System.Threading.Tasks;
using OxyPlot;
using ReactiveUI;

namespace LibrotechInspection.Desktop.ViewModels.PlotViewModels;

/// <summary>
///     PlotViewModel represents the chart View and is
///     responsible for building the chart and responding to chart events
/// </summary>
public abstract class PlotViewModel : ReactiveObject
{
    private bool _showHumidity = true;

    private bool _showPressure = true;

    private bool _showTemperature = true;

    public bool HasHumidity = false;

    public bool HasPressure = false;

    public bool HasTemperature = false;

    /// <summary>
    ///     PlotModel is the model for the chart on which the PlotView renders data.
    /// </summary>
    public abstract PlotModel? PlotModel { get; set; }

    /// <summary>
    ///     Indicate whether to build a temperature series
    /// </summary>
    public bool ShowTemperature
    {
        get => _showTemperature;
        set => this.RaiseAndSetIfChanged(ref _showTemperature, value);
    }

    /// <summary>
    ///     Indicate whether to build a humidity series
    /// </summary>
    public bool ShowHumidity
    {
        get => _showHumidity;
        set => this.RaiseAndSetIfChanged(ref _showHumidity, value);
    }

    /// <summary>
    ///     Indicate whether to build a pressure series
    /// </summary>
    public bool ShowPressure
    {
        get => _showPressure;
        set => this.RaiseAndSetIfChanged(ref _showPressure, value);
    }

    /// <summary>
    ///     BuildAsync() builds chartData for a PlotModel
    ///     instance, and creates a PlotModel instance.
    /// </summary>
    /// <param name="chartData">Data in text format</param>
    /// <returns></returns>
    public abstract Task BuildAsync(string chartData);

    /// <summary>
    ///     CreateModel() creates a PlotModel instance based on plotted data.
    ///     The data is built in the BuildAsync() method.
    /// </summary>
    /// <returns></returns>
    public abstract void CreateModel();
}