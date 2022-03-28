using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Librotech_Inspection.Models;
using ReactiveUI;

namespace Librotech_Inspection.ViewModels.Views;

public class LoggerConfigurationViewModel : ReactiveObject, IRoutableViewModel
{
    private static LoggerConfigurationViewModel? _vmInstance;

    private LoggerConfigurationViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
    }

#region Methods

    public static LoggerConfigurationViewModel GetCurrentInstance()
    {
        if (_vmInstance == null)
            throw new NullReferenceException(
                "_vmInstance cannot be null. Most likely, the GetInstance() method has never been called");

        return _vmInstance;
    }

    public static async Task<LoggerConfigurationViewModel?> CreateInstanceAsync(IScreen hostScreen, IReadableData? data)
    {
        _vmInstance = new LoggerConfigurationViewModel(hostScreen);

        if (data == null) return _vmInstance;

        if (data.DeviceSpecifications != null) _vmInstance.DeviceSpecifications = data.DeviceSpecifications.ToList();
        if (data.EmergencyEventsSettings != null)
            _vmInstance.EmergencyEventsSettings = data.EmergencyEventsSettings.ToList();
        if (data.Stamps != null) _vmInstance.Stamps = data.Stamps.ToList();

        return _vmInstance;
    }

#endregion

#region Fields

    private List<DeviceSpecification> _deviceSpecifications = new();
    private List<EmergencyEventsSettings> _emergencyEventsSettings = new();
    private List<Stamp> _stamps = new();

#endregion

#region Properies

    public List<DeviceSpecification> DeviceSpecifications
    {
        get => _deviceSpecifications;
        set => this.RaiseAndSetIfChanged(ref _deviceSpecifications, value);
    }

    public List<EmergencyEventsSettings> EmergencyEventsSettings
    {
        get => _emergencyEventsSettings;
        set => this.RaiseAndSetIfChanged(ref _emergencyEventsSettings, value);
    }

    public List<Stamp> Stamps
    {
        get => _stamps;
        set => this.RaiseAndSetIfChanged(ref _stamps, value);
    }
    
    public List<DeviceSpecification> DeviceSpecificationsPreview => _deviceSpecifications.Take(15).ToList();

    public List<EmergencyEventsSettings> EmergencyEventsSettingsPreview => _emergencyEventsSettings.Take(15).ToList();

    public List<Stamp> StampsPreview => _stamps.Take(2).ToList();

#endregion

#region IRoutableViewModel properties

    public string UrlPathSegment => "LoggerConfiguration";

    public IScreen HostScreen { get; protected set; }

#endregion
}