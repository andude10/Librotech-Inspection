using System;
using System.IO;
using System.Reactive;
using System.Windows;
using Librotech_Inspection.Interactions;
using Microsoft.Win32;

namespace Librotech_Inspection
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DialogInteractions.ShowOpenFileDialog.RegisterHandler(context =>
            {
                var openFileDialog = new OpenFileDialog();

                if (openFileDialog.ShowDialog() == true)
                {
                    context.SetOutput(openFileDialog.FileName);   
                }
            });
        }
    }
}
