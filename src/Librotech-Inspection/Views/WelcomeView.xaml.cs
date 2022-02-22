using System.Windows;
using Librotech_Inspection.ViewModels;
using ReactiveUI;

namespace Librotech_Inspection.Views;

/// <summary>
///     Interaction logic for WelcomeView.xaml
/// </summary>
public partial class WelcomeView : IViewFor<WelcomeViewModel>
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(WelcomeViewModel),
            typeof(WelcomeView),
            new PropertyMetadata(null));

    public WelcomeView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            d(this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext));
            d(this.BindCommand(ViewModel, vm => vm.NavigateToSecond, view => view.NavigateButton));
        });
    }

    public WelcomeViewModel ViewModel
    {
        get => (WelcomeViewModel) GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (WelcomeViewModel) value;
    }
}