using OxyPlot;

namespace LibrotechInspection.Desktop.Views.Controls;

/// <summary>
///     CustomPlotTrackerManipulator is a manipulator of
///     "tracker" (the tooltip that appears when you click on a point in OxyPlot)
///     whose purpose is to track the selected point on the chart.
///     (OnDeltaCompleted occurs when the selected point changes)
/// </summary>
public sealed class CustomPlotTrackerManipulator : TrackerManipulator
{
    public delegate void DeltaCompleted(OxyMouseEventArgs e);

    public CustomPlotTrackerManipulator(IPlotView plotView) : base(plotView)
    {
        Snap = true;
        PointsOnly = true;
    }

    public event DeltaCompleted? DeltaHandler;

    // Override the Completed method so that the tracker is not hidden after the button is released
    public override void Completed(OxyMouseEventArgs e)
    {
    }

    public override void Delta(OxyMouseEventArgs e)
    {
        base.Delta(e);
        e.Handled = true;

        OnDeltaCompleted(e);
    }

    private void OnDeltaCompleted(OxyMouseEventArgs e)
    {
        DeltaHandler?.Invoke(e);
    }
}