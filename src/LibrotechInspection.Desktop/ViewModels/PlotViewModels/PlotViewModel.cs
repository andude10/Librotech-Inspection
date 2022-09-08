using System.Reactive;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LibrotechInspection.Desktop.Models;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.Utilities.Json;
using OxyPlot;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LibrotechInspection.Desktop.ViewModels.PlotViewModels;

/// <summary>
///     PlotViewModel represents the chart View and is
///     responsible for building the chart and responding to chart events
/// </summary>
[JsonConverter(typeof(PlotViewModelConverter))]
public abstract class PlotViewModel : ReactiveObject
{
    public PlotViewModel(string textData)
    {
    }

    public PlotViewModel()
    {
        DisplayConditions = new DisplayConditions();
        SelectedPoint = null;
    }

    [JsonInclude] public abstract string PlotType { get; }

    [JsonInclude] public string? TextDataForPlot { get; protected set; }

    [JsonInclude] public DisplayConditions DisplayConditions { get; }

    [JsonInclude] [Reactive] public SelectedDataPoint? SelectedPoint { get; set; }

    [JsonInclude] public abstract bool HasHumidity { get; }

    [JsonInclude] public abstract bool HasPressure { get; }

    [JsonInclude] public abstract bool HasTemperature { get; }

    [JsonIgnore] public IPlotModelManager PlotModelManager { get; protected init; }

    [JsonIgnore] public IPlotController Controller { get; protected init; }

    [JsonIgnore] public abstract ReactiveCommand<Unit, Unit> MarkSelectedDataPointCommand { get; }

    public abstract Task BuildAsync();
}