using System.Reactive;
using System.Windows;
using Librotech_Inspection.Interactions;

namespace Librotech_Inspection
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            MessageInteractions.ShowMessage.RegisterHandler(context =>
            {
                MessageBox.Show(context.Input);
                context.SetOutput(Unit.Default);
            });
        }
    }
}
