using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LibrotechInspection.Core.Models;
using LibrotechInspection.Core.Models.Record;
using LibrotechInspection.Desktop.Utilities.Exceptions;
using NLog;
using ReactiveUI;
using Splat;

namespace LibrotechInspection.Desktop.ViewModels;

public class ConfigurationViewModel : ViewModelBase, IRoutableViewModel
{
    public ConfigurationViewModel(IScreen? hostScreen = null, Record? record = null)
    {
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()
            ?? throw new NoServiceFound(nameof(IScreen));
        Record = record;

        LoadRecordDataCommand = ReactiveCommand.Create(LoadRecordData);
    }

#region Commands

    [JsonIgnore] public ReactiveCommand<Unit, Unit> LoadRecordDataCommand { get; }

#endregion

#region Methods

    private void LoadRecordData()
    {
        if (Record is null)
        {
            const string message = "The data analysis process has started, although"
                                   + " there is no data to analyze, _record is null";
            Logger.Error(message);
            throw new InvalidOperationException(message);
        }

        if (Record.DeviceSpecifications is not null) DeviceSpecifications = Record.DeviceSpecifications.ToList();

        if (Record.EmergencyEventsSettings is not null)
            EmergencyEventsSettings = Record.EmergencyEventsSettings.ToList();

        if (Record.Stamps is not null) Stamps = Record.Stamps.ToList();
    }

#endregion

#region Fields

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private List<DeviceCharacteristic> _deviceSpecifications = new();
    private List<EmergencyEventsSettings> _emergencyEventsSettings = new();
    private List<Stamp> _stamps = new();

#endregion

#region Properies

    [JsonInclude] public Record? Record { get; init; }

    [JsonInclude]
    public List<DeviceCharacteristic> DeviceSpecifications
    {
        get => _deviceSpecifications;
        set => this.RaiseAndSetIfChanged(ref _deviceSpecifications, value);
    }

    [JsonInclude]
    public List<EmergencyEventsSettings> EmergencyEventsSettings
    {
        get => _emergencyEventsSettings;
        set => this.RaiseAndSetIfChanged(ref _emergencyEventsSettings, value);
    }

    [JsonInclude]
    public List<Stamp> Stamps
    {
        get => _stamps;
        set => this.RaiseAndSetIfChanged(ref _stamps, value);
    }

    [JsonIgnore] public string UrlPathSegment => "Configuration";

    [JsonIgnore] public IScreen HostScreen { get; }

#endregion
}