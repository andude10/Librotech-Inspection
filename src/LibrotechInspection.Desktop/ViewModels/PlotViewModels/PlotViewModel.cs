using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
        PlotModel = new PlotModel();

        PlotModelUpdate = this.WhenAnyValue(vm => vm.PlotModel);
        DisplayConditionsChange = DisplayConditions.WhenAnyValue(x => x.DisplayTemperature,
                x => x.DisplayHumidity,
                x => x.DisplayPressure)
            .Select(_ => Unit.Default);
    }

    [JsonInclude] public abstract string PlotType { get; }

    [JsonInclude] public string? TextDataForPlot { get; protected set; }

    [JsonInclude] public DisplayConditions DisplayConditions { get; }

    [JsonInclude] [Reactive] public ScreenPoint SelectedPoint { get; set; }

    [JsonInclude] public bool HasHumidity { get; protected set; }

    [JsonInclude] public bool HasPressure { get; protected set; }

    [JsonInclude] public bool HasTemperature { get; protected set; }

    [JsonIgnore] [Reactive] public PlotModel PlotModel { get; protected set; }

    [JsonIgnore] public IObservable<Unit> DisplayConditionsChange { get; }

    [JsonIgnore] public IObservable<PlotModel> PlotModelUpdate { get; }

    public abstract Task BuildAsync();
}