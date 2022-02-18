using System.Diagnostics;
using ReactiveUI;

namespace Librotech_Inspection.Views
{
    /// <summary>
    /// Interaction logic for SecondView.xaml
    /// </summary>
    public partial class SecondView
    {
        public SecondView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                d(this.BindCommand(ViewModel, vm => vm.Back, view => view.backButton));
                d(this.OneWayBind(ViewModel, vm => vm.MyModel, v => v.PlotView.Model));
            });
        }
    }
}
