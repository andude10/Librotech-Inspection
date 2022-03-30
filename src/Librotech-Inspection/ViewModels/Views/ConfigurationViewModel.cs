using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Librotech_Inspection.Models;
using ReactiveUI;

namespace Librotech_Inspection.ViewModels.Views;

public class ConfigurationViewModel : ReactiveObject, IRoutableViewModel
{
    private static ConfigurationViewModel? _vmInstance;

    private ConfigurationViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;

        NavigateToConfigurationDetailsCommand = ReactiveCommand.CreateFromTask<Type>(NavigateToConfigurationDetails);
    }

#region Commands

    public ReactiveCommand<Type, Unit> NavigateToConfigurationDetailsCommand { get; }

#endregion

#region Methods

    public static ConfigurationViewModel GetCurrentInstance()
    {
        if (_vmInstance == null)
            throw new NullReferenceException(
                "_vmInstance cannot be null. Most likely, the GetInstance() method has never been called");

        return _vmInstance;
    }

    public static async Task<ConfigurationViewModel?> CreateInstanceAsync(IScreen hostScreen, IReadableData? data)
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
        if (dataType == typeof(DeviceSpecification))
        {
            vm.DeviceSpecifications = DeviceSpecifications.ToList();
            await HostScreen.Router.Navigate.Execute(vm)
                .Select(_ => Unit.Default);
        }
        else if (dataType == typeof(Stamp))
        {
            vm.Stamps = Stamps.ToList();
            await HostScreen.Router.Navigate.Execute(vm)
                .Select(_ => Unit.Default);
        }
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

    public List<Stamp> StampsPreview => _stamps.Take(2).ToList();

#endregion

#region IRoutableViewModel properties

    public string UrlPathSegment => "Configuration";

    public IScreen HostScreen { get; protected set; }

#endregion
}