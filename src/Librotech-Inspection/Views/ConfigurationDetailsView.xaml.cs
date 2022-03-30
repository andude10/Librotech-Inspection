using System.Windows;
using Librotech_Inspection.ViewModels.Views;
using ReactiveUI;

namespace Librotech_Inspection.Views;

public partial class ConfigurationDetailsView : IViewFor<ConfigurationDetailsViewModel>
{
    public static readonly DependencyProperty ViewProperty =
        DependencyProperty.Register("View", typeof(ConfigurationDetailsViewModel),
            typeof(ConfigurationDetailsView),
            new PropertyMetadata(null));

    public ConfigurationDetailsView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            d(this.WhenAnyValue(x => x.View)
                .BindTo(this, x => x.DataContext));

            if (ViewModel?.DeviceSpecifications.Count != null)
            {
                d(this.OneWayBind(ViewModel, vm => vm.DeviceSpecifications,
                    view => view.DataListBox.ItemsSource));
                DataListBox.ItemTemplate = (DataTemplate) Resources["DeviceSpecificationListTemplate"];
            }

            if (ViewModel?.Stamps.Count != null)
            {
                d(this.OneWayBind(ViewModel, vm => vm.Stamps,
                    view => view.DataListBox.ItemsSource));
                DataListBox.ItemTemplate = (DataTemplate) Resources["StampListTemplate"];
            }

            d(this.BindCommand(ViewModel, vm => vm.NavigateBackCommand,
                view => view.NavigateBackButton));
        });
    }

    public ConfigurationDetailsViewModel View
    {
        get => (ConfigurationDetailsViewModel) GetValue(ViewProperty);
        set => SetValue(ViewProperty, value);
    }

    object IViewFor.ViewModel
    {
        get => View;
        set => View = (ConfigurationDetailsViewModel) value;
    }

    public ConfigurationDetailsViewModel? ViewModel { get; set; }
}