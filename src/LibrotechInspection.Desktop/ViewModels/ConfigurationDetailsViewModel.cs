using System;
using System.Reactive;
using ReactiveUI;

namespace LibrotechInspection.Desktop.ViewModels;

public class ConfigurationDetailsViewModel : ReactiveObject, IRoutableViewModel
{
#region Fields

    private object? _data;

#endregion

    public ConfigurationDetailsViewModel(IScreen hostScreen, Type dataType)
    {
        HostScreen = hostScreen;
        GoBackCommand = HostScreen.Router.NavigateBack;
        DataType = dataType;
    }

#region Commands

    public ReactiveCommand<Unit, IRoutableViewModel?> GoBackCommand { get; }

#endregion

#region Properies

    /// <summary>
    ///     Data is a list of any objects that are binds to the view.
    ///     For example: List of DeviceCharacteristic/
    ///     See ConfigurationDetailsView.xaml.cs for more information about displaying data.
    /// </summary>
    public object? Data
    {
        get => _data;
        set => this.RaiseAndSetIfChanged(ref _data, value);
    }

    public Type DataType { get; }

#endregion

#region IRoutableViewModel properties

    public string UrlPathSegment => "ConfigurationDetails";

    public IScreen HostScreen { get; protected set; }

#endregion
}