using System.Windows;
using Librotech_Inspection.ViewModels.Views;
using ReactiveUI;

namespace Librotech_Inspection.Views;

/// <summary>
///     Interaction logic for LoggerConfigurationView.xaml
/// </summary>
public partial class ConfigurationView : IViewFor<ConfigurationViewModel>
{
    public static readonly DependencyProperty ViewProperty =
        DependencyProperty.Register("View", typeof(ConfigurationViewModel),
            typeof(ConfigurationView),
            new PropertyMetadata(null));

    public ConfigurationView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            d(this.WhenAnyValue(x => x.View)
                .BindTo(this, x => x.DataContext));

            d(this.OneWayBind(ViewModel, vm => vm.EmergencyEventsSettings,
                view => view.EmergencyEventsSettingsPreviewListBox.ItemsSource));
            d(this.OneWayBind(ViewModel, vm => vm.DeviceSpecificationsPreview,
                view => view.DeviceSpecificationPreviewListBox.ItemsSource));
            d(this.OneWayBind(ViewModel, vm => vm.StampsPreview,
                view => view.StampsPreviewListBox.ItemsSource));

            d(this.BindCommand(ViewModel, vm => vm.NavigateToConfigurationDetailsCommand,
                view => view.DeviceSpecificationDetailsButton));
            d(this.BindCommand(ViewModel, vm => vm.NavigateToConfigurationDetailsCommand,
                view => view.StampsDetailsButton));
        });
    }

    public ConfigurationViewModel View
    {
        get => (ConfigurationViewModel) GetValue(ViewProperty);
        set => SetValue(ViewProperty, value);
    }

    object IViewFor.ViewModel
    {
        get => View;
        set => View = (ConfigurationViewModel) value;
    }

    public ConfigurationViewModel? ViewModel { get; set; }
}