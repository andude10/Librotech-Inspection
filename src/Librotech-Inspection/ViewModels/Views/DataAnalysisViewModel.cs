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

public class DataAnalysisViewModel : ReactiveObject, IRoutableViewModel
{
    public DataAnalysisViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
        ChartViewModel = new LineChartViewModel(new LineChartCustomizer());

        AppBootstrapper.OnDataSourceUpdated += StartAnalysisAsync;
    }

#region Methods

    /// <summary>
    ///     The StartAnalysisAsync displays a file selection dialog,
    ///     parses the data, and starts the data analysis.
    /// </summary>
    private async Task StartAnalysisAsync(IReadableData? data)
    {
        if (data == null) return;

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