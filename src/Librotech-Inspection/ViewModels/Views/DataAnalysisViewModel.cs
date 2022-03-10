using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Librotech_Inspection.Models;
using Librotech_Inspection.Utilities.ChartCustomizers;
using Librotech_Inspection.Utilities.Interactions;
using Librotech_Inspection.Utilities.Parsers.FileParsers;
using Librotech_Inspection.ViewModels.ChartViewModels;
using ReactiveUI;

namespace Librotech_Inspection.ViewModels.Views;

public class DataAnalysisViewModel : ReactiveObject, IRoutableViewModel
{
    public DataAnalysisViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;

        ChartViewModel = new LineChartViewModel(new LineChartCustomizer());

        BackCommand = HostScreen.Router.NavigateBack;
        StartAnalysisCommand = ReactiveCommand.CreateFromTask(StartAnalysis);
    }

    #region Methods

    /// <summary>
    ///     The StartAnalysisButton displays a file selection dialog,
    ///     parses the data, and starts the data analysis.
    /// </summary>
    private async Task StartAnalysis()
    {
        var path = await DialogInteractions.ShowOpenFileDialog.Handle(Unit.Default);

        if (string.IsNullOrEmpty(path))
        {
            Debug.WriteLine("The user has not selected a file for analysis");
            return;
        }

        File = await CsvFileParser.ParseAsync(path);

        if (File == null)
        {
            Debug.WriteLine("Something went wrong");
            return;
        }

        await ChartViewModel.BuildAsync(File.ChartData);
    }

    #endregion

    #region IRoutableViewModel properties

    public string UrlPathSegment => "DataAnalysis";
    public IScreen HostScreen { get; }

    #endregion

    #region Private fields

    private ChartViewModel _chartViewModel;
    private FileData? _file;

    #endregion

    #region Public properties

    public ChartViewModel ChartViewModel
    {
        get => _chartViewModel;
        private set => this.RaiseAndSetIfChanged(ref _chartViewModel, value);
    }

    public FileData? File
    {
        get => _file;
        set => this.RaiseAndSetIfChanged(ref _file, value);
    }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, IRoutableViewModel> BackCommand { get; }

    /// <summary>
    ///     The StartAnalysisCommand shows a file selection dialog,
    ///     parses the data, and starts the data analysis.
    /// </summary>
    public ReactiveCommand<Unit, Unit> StartAnalysisCommand { get; }

    #endregion
}