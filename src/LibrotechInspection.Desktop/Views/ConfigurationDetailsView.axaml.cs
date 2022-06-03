using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.ReactiveUI;
using LibrotechInspection.Core.Models;
using LibrotechInspection.Desktop.ViewModels;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Views;

public partial class ConfigurationDetailsView : ReactiveUserControl<ConfigurationDetailsViewModel>
{
    public ConfigurationDetailsView()
    {
        this.WhenActivated(d =>
        {
            // Set DataTemplate depending on data type.
            // For some reason DataType in DataTemplate doesn't work, so now it's set manually.
            // Maybe I'll fix it in the future.
            if (ViewModel != null)
            {
                if (ViewModel.Data.GetType() == typeof(List<DeviceCharacteristic>))
                    DataListBox.ItemTemplate = (DataTemplate) Resources["DeviceSpecificationListTemplate"]!;
                else if (ViewModel.Data.GetType() == typeof(List<Stamp>))
                    DataListBox.ItemTemplate = (DataTemplate) Resources["StampListTemplate"]!;
            }

            d(this.OneWayBind(ViewModel, vm => vm.Data,
                view => view.FindDataListBox.Items));

            d(this.BindCommand(ViewModel, vm => vm.GoBackCommand,
                view => view.FindGoBackButton));
        });

        AvaloniaXamlLoader.Load(this);
    }


#region Find Properties

    public ListBox FindDataListBox => this.FindControl<ListBox>("DataListBox");
    public Button FindGoBackButton => this.FindControl<Button>("GoBackButton");

#endregion
}