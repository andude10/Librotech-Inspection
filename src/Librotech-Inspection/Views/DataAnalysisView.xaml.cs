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

        this.WhenActivated(d =>
        {
            /*
            d(this.OneWayBind(ViewModel, vm => vm.HasFile,
                view => view.DropFileHere.Visibility,
                hasFile => hasFile ? Visibility.Visible : Visibility.Hidden));
            */
            d(ViewModel.UpdatePlotView.RegisterHandler(_ => { PlotView.InvalidatePlot(); }));
            d(this.BindCommand(ViewModel, vm => vm.StartAnalysisCommand,
                view => view.StartAnalysis));
            d(this.BindCommand(ViewModel, vm => vm.BackCommand,
                view => view.backButton));
            d(this.Bind(ViewModel, vm => vm.PlotModel,
                view => view.PlotView.Model));
        });
    }
}