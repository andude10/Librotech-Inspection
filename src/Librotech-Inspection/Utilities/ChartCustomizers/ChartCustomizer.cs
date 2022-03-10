using OxyPlot;

namespace Librotech_Inspection.Utilities.ChartCustomizers;

/// <summary>
///     The ChartCustomizer object customizes the entire visual part of the PlotModel.
/// </summary>
public abstract class ChartCustomizer
{
    public abstract void Customize(PlotModel plotModel);
}