using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LibrotechInspection.Desktop.ViewModels;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Views;

public partial class ConfigurationView : ReactiveUserControl<ConfigurationViewModel>
{
    public ConfigurationView()
    {
        this.WhenActivated(d =>
        {
            d(this.OneWayBind(ViewModel, vm => vm.EmergencyEventsSettings,
                view => view.FindEmergencyEventsSettingsListBox.Items));
            d(this.OneWayBind(ViewModel, vm => vm.DeviceSpecifications,
                view => view.FindDeviceSpecificationListBox.Items));
            d(this.OneWayBind(ViewModel, vm => vm.Stamps,
                view => view.FindStampsPreviewListBox.Items));
        });

        AvaloniaXamlLoader.Load(this);
    }

#region Find Properties

    public DataGrid FindEmergencyEventsSettingsListBox =>
        this.FindControl<DataGrid>(nameof(EmergencyEventsSettingsDataGrid));

    public ListBox FindDeviceSpecificationListBox =>
        this.FindControl<ListBox>(nameof(DeviceSpecificationListBox));

    public ListBox FindStampsPreviewListBox => this.FindControl<ListBox>(nameof(StampsPreviewListBox));

#endregion
}