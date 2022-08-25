using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LibrotechInspection.Desktop.ViewModels.PlotViewModels;

public class DisplayConditions : ReactiveObject
{
    [Reactive] public bool DisplayTemperature { get; set; }

    [Reactive] public bool DisplayHumidity { get; set; }

    [Reactive] public bool DisplayPressure { get; set; }
}