using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Librotech_Inspection.Models;
using Librotech_Inspection.Utilities.ChartCustomizers;
using Librotech_Inspection.Utilities.DataDecorators;
using Librotech_Inspection.Utilities.DataDecorators.Presenters;
using Librotech_Inspection.ViewModels.ChartViewModels;
using ReactiveUI;

namespace Librotech_Inspection.ViewModels.Views;

/*
 *  DataAnalysisViewModel LIFETIME
 * 
 *  The 'DataAnalysisViewModel' exists for one data source. When the
 *  data source changes, then in the 'AppBootstrapper' class, the
 *  static method 'CreateInstance()' is called which creates
 *  a new 'DataAnalysisViewModel' instance.
 *
 *  To get the current 'DataAnalysisViewModel' instance, use the 'GetCurrentInstance()' method.
 *
 *  Such a life cycle is made so that new 'DataAnalysisViewModel' instances
 *  are not created when switching tabs. If a new instance is created, it
 *  will do the work with the data again, we don't want it.
 */

// TODO: There might be a problem if multiple threads modify the instance. Need to use 'lock' or some other neat solution

/// <summary>
///     DataAnalysisViewModel is the ViewModel for the DataAnalysisView
///     DataAnalysisViewModel decorates data (for example, composes a
///     short summary based on text data), builds a chart.
///     See DataAnalysisViewModel.cs for details.
/// </summary>
public class DataAnalysisViewModel : ReactiveObject, IRoutableViewModel
{
    private static DataAnalysisViewModel? _vmInstance;
    private DataAnalysisViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
        ChartViewModel = new LineChartViewModel(new LineChartCustomizer());
    }

#region Methods

    /// <summary>
    ///     GetCurrentInstance() returns the current DataAnalysisViewModel
    ///     instance. Commonly used for navigation (see AppBootstrapper.cs)
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException">Raised when no instances have been created.</exception>
    public static DataAnalysisViewModel GetCurrentInstance()
    {
        if (_vmInstance == null)
        {
            throw new NullReferenceException(
                "_vmInstance cannot be null. Most likely, the CreateInstance() method has never been called");
        }
        
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
    public static async Task<DataAnalysisViewModel?> CreateInstanceAsync(IScreen hostScreen, IReadableData? data)
    {
        _vmInstance = new DataAnalysisViewModel(hostScreen);

        if (data == null)
        {
            return _vmInstance;
        }

        await _vmInstance.StartAnalysisAsync(data);

        return _vmInstance;
    }

    /// <summary>
    ///     StartAnalysisAsync prepares data for display, calls
    ///     the 'ChartViewModel.BuildAsync' method to build a chart.
    /// </summary>
    /// <param name="data">Read-only data for charting and other stuff</param>
    private async Task StartAnalysisAsync(IReadableData data)
    {
        await ChartViewModel.BuildAsync(data.ChartData);

        if (data.EmergencyEventsSettings != null) EmergencyEventsSettings = data.EmergencyEventsSettings.ToList();

        FileShortSummary = ShortSummaryDecorator.GenerateShortSummary(data);
    }

#endregion

#region IRoutableViewModel properties

    public string UrlPathSegment => "DataAnalysis";
    public IScreen HostScreen { get; }

#endregion

#region Fields

    private ChartViewModel _chartViewModel;
    private List<EmergencyEventsSettings> _emergencyEventsSettings = new();
    private ShortSummaryPresenter _fileShortSummary;

#endregion

#region Properties

    public ChartViewModel ChartViewModel
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

#region Commands

#endregion
    
}