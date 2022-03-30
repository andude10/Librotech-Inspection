using System.Collections.Generic;
using System.Reactive;
using Librotech_Inspection.Models;
using ReactiveUI;

namespace Librotech_Inspection.ViewModels.Views;

public class ConfigurationDetailsViewModel : ReactiveObject, IRoutableViewModel
{
    public ConfigurationDetailsViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
        NavigateBackCommand = HostScreen.Router.NavigateBack;
    }

#region Commands

    public ReactiveCommand<Unit, IRoutableViewModel> NavigateBackCommand { get; }

#endregion

#region Fields

    private List<DeviceSpecification> _deviceSpecifications = new();
    private List<Stamp> _stamps = new();

#endregion

#region Properies

    public List<DeviceSpecification> DeviceSpecifications
    {
        get => _deviceSpecifications;
        set => this.RaiseAndSetIfChanged(ref _deviceSpecifications, value);
    }

    public List<Stamp> Stamps
    {
        get => _stamps;
        set => this.RaiseAndSetIfChanged(ref _stamps, value);
    }

#endregion

#region IRoutableViewModel properties

    public string UrlPathSegment => "ConfigurationDetails";

    public IScreen HostScreen { get; protected set; }

#endregion
}