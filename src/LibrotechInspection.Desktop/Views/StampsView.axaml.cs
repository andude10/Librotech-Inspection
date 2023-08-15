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
                view => view.StampsListBox.ItemsSource));
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