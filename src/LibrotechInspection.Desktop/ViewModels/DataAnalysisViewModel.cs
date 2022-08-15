using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Models;
using LibrotechInspection.Core.Models.Record;
using LibrotechInspection.Desktop.Utilities.DataDecorators;
using LibrotechInspection.Desktop.Utilities.DataDecorators.Presenters;
using LibrotechInspection.Desktop.Utilities.Interactions;
using LibrotechInspection.Desktop.ViewModels.PlotViewModels;
using NLog;
using OxyPlot.Avalonia;
using ReactiveUI;

namespace LibrotechInspection.Desktop.ViewModels;

public class DataAnalysisViewModel : ViewModelBase, IRoutableViewModel
{
    private static DataAnalysisViewModel? _vmInstance;

    private DataAnalysisViewModel(IScreen hostScreen, IPlotCustomizer chartCustomizer)
    {
        HostScreen = hostScreen;
        PlotViewModel = new LinePlotViewModel(chartCustomizer);

        SavePlotAsFileCommand = ReactiveCommand.Create(SavePlotAsPng);
    }

#region Commands

    public ReactiveCommand<Unit, Unit> SavePlotAsFileCommand { get; }

#endregion

#region IRoutableViewModel properties

    public string UrlPathSegment => "DataAnalysis";
    public IScreen HostScreen { get; }

#endregion

#region Fields

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private PlotViewModel _plotViewModel;
    private List<EmergencyEventsSettings> _emergencyEventsSettings = new();
    private ShortSummaryPresenter _fileShortSummary;

#endregion

#region Properties

    public PlotViewModel PlotViewModel
    {
        get => _plotViewModel;
        private set => this.RaiseAndSetIfChanged(ref _plotViewModel, value);
    }

    public ShortSummaryPresenter FileShortSummary
    {
        get => _fileShortSummary;
        set => this.RaiseAndSetIfChanged(ref _fileShortSummary, value);
    }

#endregion

#region Methods

    /// <summary>
    ///     GetCurrentInstance() returns the current DataAnalysisViewModel
    ///     instance. Commonly used for navigation (see AppBootstrapper.cs)
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException">Raised when no instances have been created.</exception>
    public static DataAnalysisViewModel GetInstance()
    {
        if (_vmInstance == null)
            throw new NullReferenceException(
                "_vmInstance cannot be null. Most likely, the CreateInstanceAsync() method has never been called");

        _vmInstance.PlotViewModel.CreateModel();

        return _vmInstance;
    }

    /// <summary>
    ///     Creates a new instance of the Data Analysis ViewModel, prepares
    ///     all the data to display in the View, and build a chart.
    /// </summary>
    /// <param name="hostScreen"></param>
    /// <param name="data">Read-only data for charting and other stuff</param>
    /// <returns>The created instance</returns>
    public static async Task<DataAnalysisViewModel?> CreateInstanceAsync(IScreen hostScreen,
        Record? data, IPlotCustomizer chartCustomizer)
    {
        _vmInstance = new DataAnalysisViewModel(hostScreen, chartCustomizer);

        if (data == null) return _vmInstance;

        await _vmInstance.StartAnalysisAsync(data);

        return _vmInstance;
    }

    /// <summary>
    ///     StartAnalysisAsync prepares data for display, calls
    ///     the 'PlotViewModel.BuildAsync' method to build a chart.
    /// </summary>
    /// <param name="data">Read-only data for charting and other stuff</param>
    private async Task StartAnalysisAsync(Record data)
    {
        Logger.Info("DataAnalysisViewModel Start analysing record.");

        try
        {
            await PlotViewModel.BuildAsync(data.PlotData);
        }
        catch (Exception e)
        {
            ErrorInteractions.InnerException
                .Handle($"Произошла внутренняя ошибка во время постройки графика. Сообщение ошибки: {e.Message}")
                .Subscribe();
            throw;
        }

        FileShortSummary = ShortSummaryDecorator.GenerateShortSummary(data);
    }

    private void SavePlotAsPng()
    {
        var plotExporter = new PngExporter
        {
            Width = 600,
            Height = 400
        };

        var bitmap = plotExporter.ExportToBitmap(PlotViewModel.PlotModel);
        DialogInteractions.SaveBitmapAsPng.Handle((bitmap, "plot")).Subscribe();
    }

#endregion
}