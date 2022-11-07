using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LibrotechInspection.Desktop.ViewModels;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Views;

public partial class StampsView : ReactiveUserControl<StampsViewModel>
{
    public StampsView()
    {
        this.WhenActivated(d =>
        {
            d(this.OneWayBind(ViewModel, vm => vm.Record!.Stamps,
                view => view.FindStampsListBox.Items));
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

    public ListBox FindStampsListBox => this.FindControl<ListBox>(nameof(StampsListBox));

#endregion
}