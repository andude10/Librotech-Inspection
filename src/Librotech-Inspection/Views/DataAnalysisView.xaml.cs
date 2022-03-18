using System.Windows;
using Librotech_Inspection.Utilities.Interactions;
using ReactiveUI;

namespace Librotech_Inspection.Views;

/// <summary>
///     Interaction logic for DataAnalysisView.xaml
/// </summary>
public partial class DataAnalysisView
{
    public DataAnalysisView()
    {
        InitializeComponent();

        PlotViewInteractions.UpdatePlotView.RegisterHandler(_ => { PlotView.InvalidatePlot(); });

        this.WhenActivated(d =>
        {
            /*
            d(this.OneWayBind(ViewModel, vm => vm.HasFile,
                view => view.DropFileHere.Visibility,
                hasFile => hasFile ? Visibility.Visible : Visibility.Hidden));
            */
            d(this.BindCommand(ViewModel, vm => vm.StartAnalysisCommand,
                view => view.StartAnalysisButton));

            d(this.Bind(ViewModel, vm => vm.ChartViewModel.PlotModel,
                view => view.PlotView.Model));

            d(this.OneWayBind(ViewModel, vm => vm.File.EmergencyEventsSettings,
                view => view.EmergencyEventsListBox.ItemsSource));

            d(this.Bind(ViewModel, vm => vm.ChartViewModel.ShowTemperature,
                view => view.ShowTemperatureCheckBox.IsChecked));
            d(this.Bind(ViewModel, vm => vm.ChartViewModel.ShowHumidity,
                view => view.ShowHumidityCheckBox.IsChecked));
            d(this.Bind(ViewModel, vm => vm.ChartViewModel.ShowPressure,
                view => view.ShowPressureCheckBox.IsChecked));

            d(this.Bind(ViewModel, vm => vm.FileShortSummary,
                view => view.ShortSummaryContentPresenter.Content));
        });
    }

#region Chart manipulation methods

    private void AlignChartCenter(object sender, RoutedEventArgs e)
    {
        PlotView.ResetAllAxes();
    }

#endregion
}