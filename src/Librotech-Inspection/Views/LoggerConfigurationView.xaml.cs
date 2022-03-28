using System.Linq;
using System.Windows;
using Librotech_Inspection.ViewModels.Views;
using ReactiveUI;

namespace Librotech_Inspection.Views;

/// <summary>
///     Interaction logic for LoggerConfigurationView.xaml
/// </summary>
public partial class LoggerConfigurationView : IViewFor<LoggerConfigurationViewModel>
{
    public static readonly DependencyProperty ViewProperty =
        DependencyProperty.Register("View", typeof(LoggerConfigurationViewModel),
            typeof(LoggerConfigurationView),
            new PropertyMetadata(null));

    public LoggerConfigurationView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            d(this.WhenAnyValue(x => x.View)
                .BindTo(this, x => x.DataContext));

            d(this.OneWayBind(ViewModel, vm => vm.EmergencyEventsSettingsPreview,
                view => view.EmergencyEventsSettingsPreviewListBox.ItemsSource));

            d(this.OneWayBind(ViewModel, vm => vm.DeviceSpecificationsPreview,
                view => view.DeviceSpecificationPreviewListBox.ItemsSource));
            
            d(this.OneWayBind(ViewModel, vm => vm.StampsPreview,
                view => view.StampsPreviewListBox.ItemsSource));
        });
    }

    public LoggerConfigurationViewModel View
    {
        get => (LoggerConfigurationViewModel) GetValue(ViewProperty);
        set => SetValue(ViewProperty, value);
    }

    object IViewFor.ViewModel
    {
        get => View;
        set => View = (LoggerConfigurationViewModel) value;
    }

    public LoggerConfigurationViewModel? ViewModel { get; set; }
}