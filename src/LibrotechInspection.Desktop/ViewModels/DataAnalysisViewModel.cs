using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Models;
using LibrotechInspection.Core.Models.Record;
using LibrotechInspection.Desktop.Utilities.DataDecorators;
using LibrotechInspection.Desktop.Utilities.DataDecorators.Presenters;
using LibrotechInspection.Desktop.ViewModels.PlotViewModels;
using ReactiveUI;

namespace LibrotechInspection.Desktop.ViewModels;

public class DataAnalysisViewModel : ViewModelBase, IRoutableViewModel
{
    private static DataAnalysisViewModel? _vmInstance;

    private DataAnalysisViewModel(IScreen hostScreen, IPlotCustomizer chartCustomizer)
    {
        HostScreen = hostScreen;
        ChartViewModel = new LinePlotViewModel(chartCustomizer);
    }

#region IRoutableViewModel properties

    public string UrlPathSegment => "DataAnalysis";
    public IScreen HostScreen { get; }

#endregion

#region Fields

    private PlotViewModel _chartViewModel;
    private List<EmergencyEventsSettings> _emergencyEventsSettings = new();
    private ShortSummaryPresenter _fileShortSummary;

#endregion

#region Properties

    public PlotViewModel ChartViewModel
    {
        get => _chartViewModel;
        private set => this.RaiseAndSetIfChanged(ref _chartViewModel, value);
    }

    public ShortSummaryPresenter FileShortSummary
    {
        get => _fileShortSummary;
        set => this.RaiseAndSetIfChanged(ref _fileShortSummary, value);
    }

    public List<EmergencyEventsSettings> EmergencyEventsSettings
    {
        get => _emergencyEventsSettings;
        set => this.RaiseAndSetIfChanged(ref _emergencyEventsSettings, value);
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

        _vmInstance.ChartViewModel.CreateModel();

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
        await ChartViewModel.BuildAsync(data.PlotData);

        if (data.EmergencyEventsSettings != null) EmergencyEventsSettings = data.EmergencyEventsSettings.ToList();

        FileShortSummary = ShortSummaryDecorator.GenerateShortSummary(data);
    }

#endregion
}