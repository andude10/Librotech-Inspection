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

#region Properies

    /// <summary>
    ///     Data is a list of any objects that are binds to the view.
    ///     For example: List of DeviceSpecification/
    ///     See ConfigurationDetailsView.xaml.cs for more information about displaying data.
    /// </summary>
    public object Data
    {
        get => _data;
        set => this.RaiseAndSetIfChanged(ref _data, value);
    }

#endregion

#region Fields

    private List<DeviceSpecification> _deviceSpecifications = new();
    private List<Stamp> _stamps = new();
    private object _data;

#endregion

#region IRoutableViewModel properties

    public string UrlPathSegment => "ConfigurationDetails";

    public IScreen HostScreen { get; protected set; }

#endregion
}