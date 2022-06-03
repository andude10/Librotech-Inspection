using System.Reactive;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Utilities.Interactions;

public static class PlotViewInteractions
{
    public static readonly Interaction<Unit, Unit> UpdatePlotView = new();
}