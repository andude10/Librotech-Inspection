using System;
using System.Reactive;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LibrotechInspection.Core.Models.Record;
using LibrotechInspection.Desktop.Utilities.DataDecorators;
using LibrotechInspection.Desktop.Utilities.DataDecorators.Presenters;
using LibrotechInspection.Desktop.Utilities.Exceptions;
using LibrotechInspection.Desktop.Utilities.Interactions;
using LibrotechInspection.Desktop.ViewModels.PlotViewModels;
using LibrotechInspection.Desktop.Views.Controls;
using NLog;
using OxyPlot;
using OxyPlot.Avalonia;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace LibrotechInspection.Desktop.ViewModels;

public sealed class DataAnalysisViewModel : ViewModelBase, IRoutableViewModel
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public DataAnalysisViewModel(IScreen? hostScreen = null, Record? record = null, PlotViewModel? plotViewModel = null)
    {
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()
            ?? throw new NoServiceFound(nameof(IScreen));
        Record = record;
        PlotViewModel = plotViewModel ?? new LinePlotViewModel();
        PlotController = new PlotController();

        SetupPlotController();

        SavePlotAsFileCommand = ReactiveCommand.Create(SavePlotAsPng);
        StartAnalyseRecordCommand = ReactiveCommand.CreateFromTask(StartAnalyseRecordAsync);
    }

#region Commands

    [JsonIgnore] public ReactiveCommand<Unit, Unit> SavePlotAsFileCommand { get; }

    [JsonIgnore] public ReactiveCommand<Unit, Unit> StartAnalyseRecordCommand { get; }

#endregion

#region Properties

    [JsonInclude] [Reactive] public PlotViewModel PlotViewModel { get; set; }

    [JsonInclude] [Reactive] public ShortSummaryPresenter FileShortSummary { get; set; }

    [JsonInclude] public Record? Record { get; init; }

    [JsonIgnore] [Reactive] public IPlotController PlotController { get; set; }

    [JsonIgnore] public string UrlPathSegment => "DataAnalysis";

    [JsonIgnore] public IScreen HostScreen { get; }

#endregion

#region Methods

    private void SetupPlotController()
    {
        PlotController.BindMouseDown(OxyMouseButton.Left, new DelegatePlotCommand<OxyMouseDownEventArgs>(
            (view, controller, args) =>
            {
                var trackerManipulator = new CustomPlotTrackerManipulator(view);
                trackerManipulator.DeltaHandler += eventArgs => { PlotViewModel.SelectedPoint = eventArgs.Position; };
                controller.AddMouseManipulator(view, trackerManipulator, args);
            }));
    }

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
            PlotViewModel = new LinePlotViewModel(Record.PlotData);
            await PlotViewModel.BuildAsync();
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

        var bitmap = plotExporter.ExportToBitmap(PlotViewModel.PlotModel);
        Interactions.Dialog.SaveBitmapAsPng.Handle((bitmap, "plot")).Subscribe();
    }

#endregion
}