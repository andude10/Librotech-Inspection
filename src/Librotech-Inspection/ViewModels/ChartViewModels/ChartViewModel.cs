using System.Collections.Immutable;
using System.Threading.Tasks;
using OxyPlot;
using ReactiveUI;

namespace Librotech_Inspection.ViewModels.ChartViewModels;

/// <summary>
/// ChartViewModel represents the chart ViewModel and is
/// responsible for building the chart and responding to chart events
/// </summary>
public abstract class ChartViewModel : ReactiveObject
{
    public abstract IPlotModel PlotModel { get; set; }
    public abstract Task BuildAsync(string fileData);
}