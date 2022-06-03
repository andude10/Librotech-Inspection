using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LibrotechInspection.Core.Models;
using LibrotechInspection.Core.Models.Record;
using ReactiveUI;

namespace LibrotechInspection.Desktop.ViewModels;

public class ConfigurationViewModel : ReactiveObject, IRoutableViewModel
{
    private static ConfigurationViewModel? _vmInstance;

    private ConfigurationViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;

        GoToConfigurationDetailsCommand = ReactiveCommand.CreateFromTask<Type>(NavigateToConfigurationDetails);
    }

#region Commands

    public ReactiveCommand<Type, Unit> GoToConfigurationDetailsCommand { get; }

#endregion

#region Methods

    public static ConfigurationViewModel GetInstance()
    {
        if (_vmInstance == null)
            throw new NullReferenceException(
                "_vmInstance cannot be null. Most likely, the GetInstance() method has never been called");

        return _vmInstance;
    }

    public static async Task<ConfigurationViewModel?> CreateInstanceAsync(IScreen hostScreen, Record? data)
    {
        _vmInstance = new ConfigurationViewModel(hostScreen);

        if (data == null) return _vmInstance;

        if (data.DeviceSpecifications != null) _vmInstance.DeviceSpecifications = data.DeviceSpecifications.ToList();
        if (data.EmergencyEventsSettings != null)
            _vmInstance.EmergencyEventsSettings = data.EmergencyEventsSettings.ToList();
        if (data.Stamps != null) _vmInstance.Stamps = data.Stamps.ToList();

        return _vmInstance;
    }

    /// <summary>
    ///     Navigate to the page showing all data, base on data type
    /// </summary>
    /// <param name="dataType"></param>
    private async Task NavigateToConfigurationDetails(Type dataType)
    {
        var vm = new ConfigurationDetailsViewModel(HostScreen);
        if (dataType == typeof(DeviceCharacteristic))
        {
            await HostScreen.Router.Navigate.Execute(vm)
                .Select(_ => Unit.Default);
            vm.Data = DeviceSpecifications.ToList();
        }
        else if (dataType == typeof(Stamp))
        {
            await HostScreen.Router.Navigate.Execute(vm)
                .Select(_ => Unit.Default);
            vm.Data = Stamps.ToList();
        }
    }

#endregion

#region Fields

    private List<DeviceCharacteristic> _deviceSpecifications = new();
    private List<EmergencyEventsSettings> _emergencyEventsSettings = new();
    private List<Stamp> _stamps = new();

#endregion

#region Properies

    public List<DeviceCharacteristic> DeviceSpecifications
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

    public List<Stamp> StampsPreview => _stamps.Take(2).ToList();

#endregion

#region IRoutableViewModel properties

    public string UrlPathSegment => "Configuration";

    public IScreen HostScreen { get; protected set; }

#endregion
}