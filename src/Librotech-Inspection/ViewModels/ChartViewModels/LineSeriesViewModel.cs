using System.Threading.Tasks;
using DynamicData;
using OxyPlot;

namespace Librotech_Inspection.ViewModels.ChartViewModels;

/// <summary>
/// LineSeriesViewModel represents the chart, is responsible for plotting the chart
/// </summary>
public class LineSeriesViewModel : ChartViewModel
{
    public override IPlotModel PlotModel { get; set; }
    public override async Task BuildAsync(string path)
    {
        
    }
    
}