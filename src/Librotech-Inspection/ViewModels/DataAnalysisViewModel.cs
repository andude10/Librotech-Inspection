using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using Librotech_Inspection.Interactions;
using Librotech_Inspection.Models;
using Librotech_Inspection.Utilities.Parsers;
using Librotech_Inspection.ViewModels.ChartViewModels;
using OxyPlot;
using OxyPlot.Series;
using ReactiveUI;

namespace Librotech_Inspection.ViewModels;

public class DataAnalysisViewModel : ReactiveObject, IRoutableViewModel
{
    private ChartViewModel _chartViewModel;

    private FileData? _file;
    public FileData? File
    {
        get => _file;
        set => this.RaiseAndSetIfChanged(ref _file, value);
    }

    public bool HasFile => true;

    public string UrlPathSegment => "Second";
    public IScreen HostScreen { get; }
    
    public DataAnalysisViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
        BackCommand = HostScreen.Router.NavigateBack;
        StartAnalysisCommand = ReactiveCommand.CreateFromTask(StartAnalysis);
        _chartViewModel = new LineSeriesViewModel();
    }

    #region Commands
    
    public ReactiveCommand<Unit, IRoutableViewModel> BackCommand { get; }
    
    /// <summary>
    /// The StartAnalysisCommand shows a file selection dialog,
    /// parses the data, and starts the data analysis.
    /// </summary>
    public ReactiveCommand<Unit, Unit> StartAnalysisCommand { get; }
    #endregion

    #region Methods

    /// <summary>
    /// The StartAnalysis displays a file selection dialog,
    /// parses the data, and starts the data analysis.
    /// </summary>
    public async Task StartAnalysis()
    {
        var path = await DialogInteractions.ShowOpenFileDialog.Handle(Unit.Default);

        if (string.IsNullOrEmpty(path))
        {
            Debug.WriteLine("The user has not selected a file for analysis");
            return;
        }

        File = await CsvFileParser.ParseAsync(path);
        
        await _chartViewModel.BuildAsync(File.ChartData);
    }

    #endregion
}