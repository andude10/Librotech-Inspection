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
                FindShowTemperatureCheckBox.IsVisible = ViewModel.LinePlotViewModel.HasTemperature;
                FindShowHumidityCheckBox.IsVisible = ViewModel.LinePlotViewModel.HasHumidity;
                FindShowPressureCheckBox.IsVisible = ViewModel.LinePlotViewModel.HasPressure;
            }

            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.ModelManager.PlotModel,
                view => view.FindPlotView.Model));
            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.Controller,
                view => view.FindPlotView.Controller));

            d(this.BindCommand(ViewModel, vm => vm.SavePlotAsFileCommand,
                view => view.FindPlotSavePlotFlyoutItem));

            d(this.BindCommand(ViewModel, vm => vm.LinePlotViewModel.ZoomInCommand,
                view => view.FindZoomInButton));
            d(this.BindCommand(ViewModel, vm => vm.LinePlotViewModel.ZoomOutCommand,
                view => view.FindZoomOutButton));

            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.DisplayConditions.DisplayTemperature,
                view => view.FindShowTemperatureCheckBox.IsChecked));
            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.DisplayConditions.DisplayHumidity,
                view => view.FindShowHumidityCheckBox.IsChecked));
            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.DisplayConditions.DisplayPressure,
                view => view.FindShowPressureCheckBox.IsChecked));

            d(this.OneWayBind(ViewModel, vm => vm.Record!.DeviceSpecifications, 
                view => view.FindDeviceSpecificationListBox.Items));

            SelectSelectionZoomPlotTool(null, null);
        });

        AvaloniaXamlLoader.Load(this);
    }

#region Methods

    private void AlignChartCenter(object sender, RoutedEventArgs e)
    {
        var plotView = FindPlotView;
        foreach (var axis in plotView.Model.Axes) axis.Reset();

        plotView.InvalidatePlot();
    }

    private void MinimizeSidePanel(object? sender, RoutedEventArgs e)
    {
        FindSidePanelSectionGrid.IsVisible = !FindSidePanelSectionGrid.IsVisible;
    }

    private void SelectSelectionZoomPlotTool(object? sender, RoutedEventArgs? e)
    {
        if (ViewModel is null) return;

        ViewModel.LinePlotViewModel.SelectedTool = PlotTool.SelectionZoom;

        FindSelectSelectionZoomButton.Classes.Add("highlighted-button");
        FindSelectPanningButton.Classes.Remove("highlighted-button");
    }

    private void SelectPanningPlotTool(object? sender, RoutedEventArgs? e)
    {
        if (ViewModel is null) return;

        ViewModel.LinePlotViewModel.SelectedTool = PlotTool.Panning;

        FindSelectPanningButton.Classes.Add("highlighted-button");
        FindSelectSelectionZoomButton.Classes.Remove("highlighted-button");
    }

#endregion

#region Find Properties

    public Grid FindSidePanelSectionGrid => this.FindControl<Grid>(nameof(SidePanelSectionGrid));

    public PlotView FindPlotView => this.FindControl<PlotView>(nameof(PlotView));

    public MenuItem FindPlotSavePlotFlyoutItem =>
        this.FindControl<MenuItem>(nameof(PlotSavePlotFlyoutItem));

    public Button FindSelectSelectionZoomButton => this.FindControl<Button>(nameof(SelectSelectionZoomButton));
    public Button FindSelectPanningButton => this.FindControl<Button>(nameof(SelectPanningButton));
    public Button FindZoomInButton => this.FindControl<Button>(nameof(ZoomInButton));
    public Button FindZoomOutButton => this.FindControl<Button>(nameof(ZoomOutButton));

    public CheckBox FindShowTemperatureCheckBox => this.FindControl<CheckBox>(nameof(ShowTemperatureCheckBox));
    public CheckBox FindShowHumidityCheckBox => this.FindControl<CheckBox>(nameof(ShowHumidityCheckBox));
    public CheckBox FindShowPressureCheckBox => this.FindControl<CheckBox>(nameof(ShowPressureCheckBox));

    public ListBox FindDeviceSpecificationListBox =>
        this.FindControl<ListBox>(nameof(DeviceSpecificationListBox));

#endregion
}