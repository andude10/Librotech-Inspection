using System.Reactive;
using System.Windows;
using Librotech_Inspection.Utilities.Interactions;
using Microsoft.Win32;

namespace Librotech_Inspection;

public partial class App : Application
{ 
    public App()
    {
        DialogInteractions.ShowOpenFileDialog.RegisterHandler(context =>
        {
            var openFileDialog = new OpenFileDialog();
            context.SetOutput(openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null);
        });

        ErrorInteractions.Error.RegisterHandler(context =>
        {
            MessageBox.Show(context.Input, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            context.SetOutput(Unit.Default);
        });
    }
}