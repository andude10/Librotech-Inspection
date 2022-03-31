using System.Collections.Generic;
using System.Windows;
using Librotech_Inspection.Models;
using ReactiveUI;

namespace Librotech_Inspection.Views;

/// <summary>
///     The ConfigurationDetailsView is a view for displaying any type of data.
/// </summary>
/*
 *  Usually, the data (data in this context is, for example,
 *  the configuration of the logger) is not displayed all
 *  (only a preview of the data), so as to to view the data,
 *  this view is used.
 *
 *  The data output format is set in the DataTemplate
 *  in the ConfigurationDetailsView resources.
 */
public partial class ConfigurationDetailsView
{
    public ConfigurationDetailsView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            // Set DataTemplate depending on data type.
            // For some reason DataType in DataTemplate doesn't work, so now it's set manually.
            // Maybe I'll fix it in the future.
            if (ViewModel != null)
            {
                if (ViewModel.Data.GetType() == typeof(List<DeviceSpecification>))
                    DataListBox.ItemTemplate = (DataTemplate) Resources["DeviceSpecificationListTemplate"];
                else if (ViewModel.Data.GetType() == typeof(List<Stamp>))
                    DataListBox.ItemTemplate = (DataTemplate) Resources["StampListTemplate"];
            }

            d(this.OneWayBind(ViewModel, vm => vm.Data,
                view => view.DataListBox.ItemsSource));

            d(this.BindCommand(ViewModel, vm => vm.NavigateBackCommand,
                view => view.NavigateBackButton));
        });
    }
}