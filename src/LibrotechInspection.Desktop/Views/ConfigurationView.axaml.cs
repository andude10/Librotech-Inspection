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
                view => view.FindEmergencyEventsSettingsPreviewListBox.Items));
            d(this.OneWayBind(ViewModel, vm => vm.DeviceSpecifications,
                view => view.FindDeviceSpecificationPreviewListBox.Items));
            d(this.OneWayBind(ViewModel, vm => vm.StampsPreview,
                view => view.FindStampsPreviewListBox.Items));

            d(this.BindCommand(ViewModel, vm => vm.GoToConfigurationDetailsCommand,
                view => view.FindStampsDetailsButton));
        });

        AvaloniaXamlLoader.Load(this);
    }

#region Find Properties

    public ListBox FindEmergencyEventsSettingsPreviewListBox =>
        this.FindControl<ListBox>("EmergencyEventsSettingsPreviewListBox");

    public ListBox FindDeviceSpecificationPreviewListBox =>
        this.FindControl<ListBox>("DeviceSpecificationPreviewListBox");

    public ListBox FindStampsPreviewListBox => this.FindControl<ListBox>("StampsPreviewListBox");
    public Button FindStampsDetailsButton => this.FindControl<Button>("StampsDetailsButton");

#endregion
}