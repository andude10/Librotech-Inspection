using System;
using System.Linq;
using System.Text.Json.Serialization;
using LibrotechInspection.Core.Models.Record;
using LibrotechInspection.Desktop.Utilities.Exceptions;
using ReactiveUI;
using Splat;

namespace LibrotechInspection.Desktop.ViewModels;

public class DeviceAlarmSettingsViewModel : ViewModelBase, IRoutableViewModel
{
    public DeviceAlarmSettingsViewModel(Record record, IScreen? hostScreen = null)
    {
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()
            ?? throw new NoServiceFound(nameof(IScreen));
        Record = record;
    }
    
#region Properies
    
    [JsonInclude] public Record? Record { get; init; }
    
    [JsonIgnore] public string UrlPathSegment => "AlarmSettings";

    [JsonIgnore] public IScreen HostScreen { get; }
    
#endregion
}