using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LibrotechInspection.Desktop.Models;

public class DisplayConditions : ReactiveObject
{
    public DisplayConditions()
    {
        DisplayTemperature = true;
        DisplayHumidity = true;
        DisplayPressure = true;
    }

    [Reactive] public bool DisplayTemperature { get; set; }

    [Reactive] public bool DisplayHumidity { get; set; }

    [Reactive] public bool DisplayPressure { get; set; }
}