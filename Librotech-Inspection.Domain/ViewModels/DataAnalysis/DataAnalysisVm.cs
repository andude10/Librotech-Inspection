using System.Collections.ObjectModel;
using ReactiveUI;
using DynamicData;
using LiveChartsCore;

namespace Librotech_Inspection.Domain.ViewModels.DataAnalysis;

public class DataAnalysisVm : BaseVm
{
    private readonly ReadOnlyObservableCollection<ISeries> _series;
    public ReadOnlyObservableCollection<ISeries> Series => _series;

    public void Initialize()
    {
        
    }
}