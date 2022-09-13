using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LibrotechInspection.Core.Models.Record;
using LibrotechInspection.Desktop.Utilities.DataDecorators;
using LibrotechInspection.Desktop.Utilities.DataDecorators.Presenters;
using LibrotechInspection.Desktop.Utilities.Exceptions;
using LibrotechInspection.Desktop.Utilities.Interactions;
using LibrotechInspection.Desktop.ViewModels.PlotViewModels;
using NLog;
using OxyPlot.Avalonia;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using DateTimeAxis = OxyPlot.Axes.DateTimeAxis;

namespace LibrotechInspection.Desktop.ViewModels;

public sealed class DataAnalysisViewModel : ViewModelBase, IRoutableViewModel
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public DataAnalysisViewModel(IScreen? hostScreen = null, Record? record = null,
        LinePlotViewModelBase? linePlotViewModel = null)
    {
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()
            ?? throw new NoServiceFound(nameof(IScreen));
        Record = record;
        LinePlotViewModel = linePlotViewModel ?? new LinePlotViewModel();

        SavePlotAsFileCommand = ReactiveCommand.Create(SavePlotAsPng);
        StartAnalyseRecordCommand = ReactiveCommand.CreateFromTask(StartAnalyseRecordAsync);

        LinePlotViewModel.WhenAnyValue(vm => vm.SelectedPoint)
            .WhereNotNull()
            .Select(selectedPoint => new SelectedPointOnPlotInfo(
                DateTimeAxis.ToDateTime(selectedPoint.Point.X).ToString(),
                selectedPoint.Point.Y.ToString(),
                selectedPoint.ParentElement.ToString()))
            .Subscribe(info => SelectedPointInfo = info);
    }

#region Commands

    [JsonIgnore] public ReactiveCommand<Unit, Unit> SavePlotAsFileCommand { get; }

    [JsonIgnore] public ReactiveCommand<Unit, Unit> StartAnalyseRecordCommand { get; }

#endregion

#region Properties

    [JsonInclude] [Reactive] public LinePlotViewModelBase LinePlotViewModel { get; set; }

    [JsonInclude] [Reactive] public ShortSummaryPresenter FileShortSummary { get; set; }

    [JsonInclude] [Reactive] public SelectedPointOnPlotInfo SelectedPointInfo { get; set; }

    [JsonInclude] public Record? Record { get; init; }

    [JsonIgnore] public string UrlPathSegment => "DataAnalysis";

    [JsonIgnore] public IScreen HostScreen { get; }

#endregion

#region Methods

    /// <summary>
    ///     StartAnalysisAsync prepares data for display, calls
    ///     the 'PlotViewModel.BuildAsync' method to build a chart.
    /// </summary>
    private async Task StartAnalyseRecordAsync()
    {
        if (Record is null)
        {
            const string message = "The data analysis process has started, although"
                                   + " there is no data to analyze, _record is null";
            Logger.Error(message);
            throw new InvalidOperationException(message);
        }

        Logger.Info("DataAnalysisViewModel Start analysing record.");

        try
        {
            LinePlotViewModel = new LinePlotViewModel(Record.PlotData);
            await LinePlotViewModel.BuildAsync();
        }
        catch (Exception e)
        {
            Interactions.Error.InnerException
                .Handle($"Произошла внутренняя ошибка во время постройки графика. Сообщение ошибки: {e.Message}")
                .Subscribe();
            throw;
        }

        FileShortSummary = ShortSummaryDecorator.GenerateShortSummary(Record);
    }

    private void SavePlotAsPng()
    {
        var plotExporter = new PngExporter
        {
            Width = 600,
            Height = 400
        };

        var bitmap = plotExporter.ExportToBitmap(LinePlotViewModel.ModelManager.PlotModel);
        Interactions.Dialog.SaveBitmapAsPng.Handle((bitmap, "plot")).Subscribe();
    }

#endregion
}