using System;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using ReactiveUI;
using Splat;

namespace Librotech_Inspection.ViewModels.MainPages;

public class DataAnalysisVm : ReactiveObject, IRoutableViewModel
{
    private readonly ReadOnlyObservableCollection<ISeries> _series;
    public ReadOnlyObservableCollection<ISeries> Series => _series;
    public string UrlPathSegment => "DataAnalysis";
    public IScreen HostScreen { get; }
    
    public DataAnalysisVm()
    {
        _series = new ReadOnlyObservableCollection<ISeries>(new ObservableCollection<ISeries>()
        {
            new LineSeries<int>
            {
                Values = new [] { 4, 4, 7, 2, 8 },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null
            },
            new LineSeries<int>
            {
                Values = new [] { 7, 5, 3, 2, 6 },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null
            },
            new LineSeries<int>
            {
                Values = new [] { 4, 2, 5, 3, 9 },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null
            }
        });
        HostScreen = Locator.Current.GetService<IScreen>() ?? throw new InvalidOperationException();
    }
}