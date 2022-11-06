using Avalonia;
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
            d(this.OneWayBind(ViewModel, vm => vm.Record.DeviceAlarmSettings,
                view => view.FindAlarmSettingsDataGrid.Items));
            d(this.OneWayBind(ViewModel, vm => vm.Record!.DeviceSpecifications, 
                view => view.FindDeviceSpecificationListBox.Items));
        });
        AvaloniaXamlLoader.Load(this);
    }
    
#region Methods

    private void MinimizeSidePanel(object? sender, RoutedEventArgs e)
    {
        FindSidePanelSectionGrid.IsVisible = !FindSidePanelSectionGrid.IsVisible;
    }
    
#endregion
    
#region Find Properties
    
    public Grid FindSidePanelSectionGrid => this.FindControl<Grid>(nameof(SidePanelSectionGrid));
    
    public ListBox FindDeviceSpecificationListBox =>
        this.FindControl<ListBox>(nameof(DeviceSpecificationListBox));
    
    public DataGrid FindAlarmSettingsDataGrid => this.FindControl<DataGrid>(nameof(AlarmSettingsDataGrid));
    
#endregion
}