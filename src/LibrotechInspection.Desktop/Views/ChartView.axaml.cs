using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LibrotechInspection.Desktop.Utilities.Enums;
using LibrotechInspection.Desktop.ViewModels;
using OxyPlot.Avalonia;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Views;

public partial class ChartView : ReactiveUserControl<ChartViewModel>
{
    public ChartView()
    {
        this.WhenActivated(d =>
        {
            if (ViewModel != null)
            {
                ShowTemperatureCheckBox.IsVisible = ViewModel.LinePlotViewModel.HasTemperature;
                ShowHumidityCheckBox.IsVisible = ViewModel.LinePlotViewModel.HasHumidity;
                ShowPressureCheckBox.IsVisible = ViewModel.LinePlotViewModel.HasPressure;

                if (ViewModel.Record is null)
                {
                    DisablePlotViewFlyoutItems();
                }
            }

            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.ModelManager.PlotModel,
                view => view.PlotView.Model));
            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.Controller,
                view => view.PlotView.Controller));

            d(this.BindCommand(ViewModel, vm => vm.SavePlotAsFileCommand,
                view => view.PlotSavePlotFlyoutItem));

            d(this.BindCommand(ViewModel, vm => vm.LinePlotViewModel.ZoomInCommand,
                view => view.ZoomInButton));
            d(this.BindCommand(ViewModel, vm => vm.LinePlotViewModel.ZoomOutCommand,
                view => view.ZoomOutButton));

            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.DisplayConditions.DisplayTemperature,
                view => view.ShowTemperatureCheckBox.IsChecked));
            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.DisplayConditions.DisplayHumidity,
                view => view.ShowHumidityCheckBox.IsChecked));
            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.DisplayConditions.DisplayPressure,
                view => view.ShowPressureCheckBox.IsChecked));

            d(this.OneWayBind(ViewModel, vm => vm.Record!.DeviceSpecifications, 
                view => view.DeviceSpecificationListBox.ItemsSource));

            SelectSelectionZoomPlotTool(null, null);
        });

        AvaloniaXamlLoader.Load(this);
        InitializeComponent();
    }

#region Methods

    private void AlignChartCenter(object sender, RoutedEventArgs e)
    {
        var plotView = PlotView;
        foreach (var axis in plotView.Model.Axes) axis.Reset();

        plotView.InvalidatePlot();
    }

    private void MinimizeSidePanel(object? sender, RoutedEventArgs e)
    {
        SidePanelSectionGrid.IsVisible = !SidePanelSectionGrid.IsVisible;
    }

    private void SelectSelectionZoomPlotTool(object? sender, RoutedEventArgs? e)
    {
        if (ViewModel is null) return;

        ViewModel.LinePlotViewModel.SelectedTool = PlotTool.SelectionZoom;

        SelectSelectionZoomButton.Classes.Add("highlighted-button");
        SelectPanningButton.Classes.Remove("highlighted-button");
    }

    private void SelectPanningPlotTool(object? sender, RoutedEventArgs? e)
    {
        if (ViewModel is null) return;

        ViewModel.LinePlotViewModel.SelectedTool = PlotTool.Panning;

        SelectPanningButton.Classes.Add("highlighted-button");
        SelectSelectionZoomButton.Classes.Remove("highlighted-button");
    }

    private void DisablePlotViewFlyoutItems()
    {
        if (PlotView.ContextFlyout is not MenuFlyout menuFlyout)
        {
            throw new InvalidOperationException($"ContextFlyout in PlotView is of type" +
                                                $" {PlotView.ContextFlyout?.GetType()}, expected: {typeof(MenuFlyout)}");
        }

        foreach (var item in menuFlyout.Items)
        {
            if (item is not MenuItem menuItem)
            {
                throw new InvalidOperationException($"Item in {typeof(MenuItem)} is of type" +
                                                    $" {item.GetType()}, expected: {typeof(MenuItem)}");
            }

            menuItem.IsVisible = false;
        }
    }

#endregion
}