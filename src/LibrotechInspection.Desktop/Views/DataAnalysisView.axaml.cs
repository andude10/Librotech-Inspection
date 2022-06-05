using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LibrotechInspection.Desktop.Utilities.Interactions;
using LibrotechInspection.Desktop.ViewModels;
using OxyPlot.Avalonia;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Views;

public partial class DataAnalysisView : ReactiveUserControl<DataAnalysisViewModel>
{
    public DataAnalysisView()
    {
        PlotViewInteractions.UpdatePlotView.RegisterHandler(_ => { PlotView.InvalidatePlot(); });

        this.WhenActivated(d =>
        {
            d(this.Bind(ViewModel, vm => vm.ChartViewModel.PlotModel,
                view => view.FindPlotView.Model));

            d(this.OneWayBind(ViewModel, vm => vm.EmergencyEventsSettings,
                view => view.FindEmergencyEventsListBox.Items));

            d(this.Bind(ViewModel, vm => vm.ChartViewModel.ShowTemperature,
                view => view.FindShowTemperatureCheckBox.IsChecked));
            d(this.Bind(ViewModel, vm => vm.ChartViewModel.ShowHumidity,
                view => view.FindShowHumidityCheckBox.IsChecked));
            d(this.Bind(ViewModel, vm => vm.ChartViewModel.ShowPressure,
                view => view.FindShowPressureCheckBox.IsChecked));

            d(this.Bind(ViewModel, vm => vm.FileShortSummary,
                view => view.FindShortSummaryContentPresenter.Content));
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

#endregion

#region Find Properties

    public PlotView FindPlotView => this.FindControl<PlotView>("PlotView");
    public ListBox FindEmergencyEventsListBox => this.FindControl<ListBox>("EmergencyEventsListBox");
    public CheckBox FindShowTemperatureCheckBox => this.FindControl<CheckBox>("ShowTemperatureCheckBox");
    public CheckBox FindShowHumidityCheckBox => this.FindControl<CheckBox>("ShowHumidityCheckBox");
    public CheckBox FindShowPressureCheckBox => this.FindControl<CheckBox>("ShowPressureCheckBox");

    public ContentPresenter FindShortSummaryContentPresenter =>
        this.FindControl<ContentPresenter>("ShortSummaryContentPresenter");

#endregion
}