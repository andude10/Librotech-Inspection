using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LibrotechInspection.Desktop.ViewModels;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Views;

public partial class DeviceAlarmSettingsView : ReactiveUserControl<DeviceAlarmSettingsViewModel>
{
    public DeviceAlarmSettingsView()
    {
        this.WhenActivated(d =>
        {
            d(this.OneWayBind(ViewModel, vm => vm.Record!.DeviceAlarmSettings,
                view => view.AlarmSettingsDataGrid.ItemsSource));
            d(this.OneWayBind(ViewModel, vm => vm.Record!.DeviceSpecifications,
                view => view.DeviceSpecificationListBox.ItemsSource));
        });
        AvaloniaXamlLoader.Load(this);
        InitializeComponent();
    }

#region Methods

    private void MinimizeSidePanel(object? sender, RoutedEventArgs e)
    {
        SidePanelSectionGrid.IsVisible = !SidePanelSectionGrid.IsVisible;
    }

#endregion
}