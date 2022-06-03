using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Services.CsvFileParser;
using LibrotechInspection.Core.Services.CsvPlotDataParser;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.Utilities.Interactions;
using LibrotechInspection.Desktop.ViewModels;
using LibrotechInspection.Desktop.Views;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using ReactiveUI;
using Splat;

namespace LibrotechInspection.Desktop;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void RegisterServices()
    {
        // register views
        Locator.CurrentMutable.Register(() => new DataAnalysisView(), typeof(IViewFor<DataAnalysisViewModel>));
        Locator.CurrentMutable.Register(() => new ConfigurationView(), typeof(IViewFor<ConfigurationViewModel>));
        Locator.CurrentMutable.Register(() => new ConfigurationView(), typeof(IViewFor<ConfigurationViewModel>));

        // Services registration
        Locator.CurrentMutable.Register(() => new CsvFileParser(), typeof(IFileRecordParser));
        Locator.CurrentMutable.Register(() => new CsvPlotDataParser(), typeof(IPlotDataParser));
        Locator.CurrentMutable.Register(() => new LinePlotCustomizer(), typeof(IPlotCustomizer));
        
        DialogInteractions.ShowOpenFileDialog.RegisterHandler(async context =>
        {
            if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
            var openFileDialog = new OpenFileDialog();

            var result = await openFileDialog.ShowAsync(desktop.MainWindow);
            
            context.SetOutput(result?.First());
        });

        ErrorInteractions.Error.RegisterHandler(context =>
        {
            var messageBox =
                MessageBoxManager.GetMessageBoxStandardWindow("Ошибка", context.Input, ButtonEnum.Ok, Icon.Error);
            messageBox.Show();
            context.SetOutput(Unit.Default);
        });
        
        base.RegisterServices();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };

        base.OnFrameworkInitializationCompleted();
    }
}