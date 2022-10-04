using System;
using System.Reactive.Linq;
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
                FindShowTemperatureCheckBox.IsEnabled = ViewModel.LinePlotViewModel.HasTemperature;
                FindShowHumidityCheckBox.IsEnabled = ViewModel.LinePlotViewModel.HasHumidity;
                FindShowPressureCheckBox.IsEnabled = ViewModel.LinePlotViewModel.HasPressure;
            }

            d(this.WhenAnyObservable(vm => vm.ViewModel!.LinePlotViewModel.SelectedPointObservable)
                .Select(point => point is not null)
                .Subscribe(pointNotNull =>
                {
                    FindPlotMarkPointFlyoutItem.IsVisible = pointNotNull;
                    FindPlotCreateSeparatorLineFlyoutItem.IsVisible = pointNotNull;
                }));

            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.ModelManager.PlotModel,
                view => view.FindPlotView.Model));
            d(this.Bind(ViewModel, vm => vm.LinePlotViewModel.Controller,
                view => view.FindPlotView.Controller));

            d(this.BindCommand(ViewModel, vm => vm.SavePlotAsFileCommand,
                view => view.FindPlotSavePlotFlyoutItem));
            d(this.BindCommand(ViewModel, vm => vm.LinePlotViewModel.MarkSelectedPointCommand,
                view => view.FindPlotMarkPointFlyoutItem));
            d(this.BindCommand(ViewModel, vm => vm.LinePlotViewModel.CreateSeparatorLineCommand,
                view => view.FindPlotCreateSeparatorLineFlyoutItem));
            d(this.BindCommand(ViewModel, vm => vm.LinePlotViewModel.ClearAnnotationsCommand,
                view => view.FindPlotClearAnnotationsFlyoutItem));

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

            d(this.Bind(ViewModel, vm => vm.FileShortSummary,
                view => view.FindShortSummaryContentControl.Content));

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

    public MenuItem FindPlotSavePlotFlyoutItem=>
        this.FindControl<MenuItem>(nameof(PlotSavePlotFlyoutItem));
    public MenuItem FindPlotClearAnnotationsFlyoutItem =>
        this.FindControl<MenuItem>(nameof(PlotClearAnnotationsFlyoutItem));
    public MenuItem FindPlotMarkPointFlyoutItem => this.FindControl<MenuItem>(nameof(PlotMarkPointFlyoutItem));
    public MenuItem FindPlotCreateSeparatorLineFlyoutItem =>
        this.FindControl<MenuItem>(nameof(PlotCreateSeparatorLineFlyoutItem));

    public Button FindSelectSelectionZoomButton => this.FindControl<Button>(nameof(SelectSelectionZoomButton));
    public Button FindSelectPanningButton => this.FindControl<Button>(nameof(SelectPanningButton));
    public Button FindZoomInButton => this.FindControl<Button>(nameof(ZoomInButton));
    public Button FindZoomOutButton => this.FindControl<Button>(nameof(ZoomOutButton));

    public CheckBox FindShowTemperatureCheckBox => this.FindControl<CheckBox>(nameof(ShowTemperatureCheckBox));
    public CheckBox FindShowHumidityCheckBox => this.FindControl<CheckBox>(nameof(ShowHumidityCheckBox));
    public CheckBox FindShowPressureCheckBox => this.FindControl<CheckBox>(nameof(ShowPressureCheckBox));

    public ContentControl FindShortSummaryContentControl =>
        this.FindControl<ContentControl>(nameof(ShortSummaryContentControl));

#endregion
}