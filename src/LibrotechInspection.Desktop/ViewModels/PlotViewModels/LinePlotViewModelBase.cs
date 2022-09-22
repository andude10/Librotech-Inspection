using System;
using System.Reactive;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LibrotechInspection.Desktop.Models;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.Utilities.Enums;
using LibrotechInspection.Desktop.Utilities.Json;
using OxyPlot;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LibrotechInspection.Desktop.ViewModels.PlotViewModels;

[JsonConverter(typeof(LinePlotViewModelBaseConverter))]
public abstract class LinePlotViewModelBase : ReactiveObject
{
    public LinePlotViewModelBase(string textData)
    {
    }

    protected LinePlotViewModelBase()
    {
        DisplayConditions = new DisplayConditions();
        ModelManager = new LinePlotModelManager();
        Controller = new PlotController();
        SelectedPoint = null;

        SelectedPointObservable = this.WhenAnyValue(vm => vm.SelectedPoint);
        SelectedToolObservable = this.WhenAnyValue(vm => vm.SelectedTool);
    }

    [JsonInclude] public abstract string PlotType { get; }

    [JsonInclude] public string? TextDataForPlot { get; protected set; }

    [JsonInclude] public DisplayConditions DisplayConditions { get; }

    [JsonInclude] public abstract bool HasHumidity { get; }

    [JsonInclude] public abstract bool HasPressure { get; }

    [JsonInclude] public abstract bool HasTemperature { get; }

    [JsonInclude] public LinePlotModelManager ModelManager { get; protected init; }

    [JsonIgnore] public IPlotController Controller { get; protected init; }

    [JsonIgnore] [Reactive] public PlotTool SelectedTool { get; set; }

    [JsonIgnore] [Reactive] public SelectedDataPoint? SelectedPoint { get; set; }

    [JsonIgnore] public abstract ReactiveCommand<Unit, Unit> MarkSelectedPointCommand { get; }

    [JsonIgnore] public abstract ReactiveCommand<Unit, Unit> CreateSeparatorLineCommand { get; }

    [JsonIgnore] public abstract ReactiveCommand<Unit, Unit> ZoomInCommand { get; }

    [JsonIgnore] public abstract ReactiveCommand<Unit, Unit> ZoomOutCommand { get; }
    
    [JsonIgnore] public abstract ReactiveCommand<Unit, Unit> ClearAnnotationsCommand { get; }

    [JsonIgnore] public IObservable<SelectedDataPoint?> SelectedPointObservable { get; }

    [JsonIgnore] public IObservable<PlotTool> SelectedToolObservable { get; }

    public abstract Task BuildAsync();
}