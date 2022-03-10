using System.Reactive;
using ReactiveUI;

namespace Librotech_Inspection.Utilities.Interactions;

public static class PlotViewInteractions
{
    public static readonly Interaction<Unit, Unit> UpdatePlotView = new();
}