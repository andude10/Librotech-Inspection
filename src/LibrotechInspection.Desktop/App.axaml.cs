using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Services;
using LibrotechInspection.Core.Services.CsvFileParser;
using LibrotechInspection.Core.Services.CsvPlotDataParser;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.Utilities.Interactions;
using LibrotechInspection.Desktop.ViewModels;
using LibrotechInspection.Desktop.Views;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using NLog;
using ReactiveUI;
using Splat;
using Splat.NLog;
using ILogger = Splat.ILogger;
using LogLevel = NLog.LogLevel;

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
        Locator.CurrentMutable.Register(() => new ConfigurationDetailsView(),
            typeof(IViewFor<ConfigurationDetailsViewModel>));

        // Services registration
        Locator.CurrentMutable.Register(() => new DebugLogger(), typeof(ILogger));
        Locator.CurrentMutable.Register(() => new CsvFileParser(), typeof(IFileRecordParser));
        Locator.CurrentMutable.Register(() => new CsvPlotDataParser(), typeof(IPlotDataParser));
        Locator.CurrentMutable.Register(() => new LinePlotCustomizer(), typeof(IPlotCustomizer));
        Locator.CurrentMutable.Register(() => new DouglasPeuckerOptimizer(), typeof(ILinePlotOptimizer));
        Locator.CurrentMutable.UseNLogWithWrappingFullLogger();

        RegisterInteractionsHandlers();
        ConfigureNLog();

        base.RegisterServices();
    }

    private void RegisterInteractionsHandlers()
    {
        Interactions.Dialog.SaveTextFileDialog.RegisterHandler(async context =>
        {
            if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

            var saveFileDialog = new SaveFileDialog
            {
                DefaultExtension = ".txt"
            };

            var result = await saveFileDialog.ShowAsync(desktop.MainWindow);

            context.SetOutput(result);
        });

        Interactions.Dialog.ShowOpenFileDialog.RegisterHandler(async context =>
        {
            if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
            var openFileDialog = new OpenFileDialog();

            var result = await openFileDialog.ShowAsync(desktop.MainWindow);

            context.SetOutput(result?.First());
        });

        Interactions.Dialog.SaveBitmapAsPng.RegisterHandler(async context =>
        {
            if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

            var dialogFilters = new List<FileDialogFilter>
            {
                new()
                {
                    Name = ".png",
                    Extensions = {"png"}
                }
            };
            var saveFileDialog = new SaveFileDialog
            {
                InitialFileName = context.Input.Item2,
                Filters = dialogFilters
            };

            var path = await saveFileDialog.ShowAsync(desktop.MainWindow);
            if (path == null || !path.Contains(".png")) return;

            var bitmap = context.Input.Item1;

            await using var stream = File.Create(path);
            bitmap.Save(stream);
        });

        Interactions.Error.ExternalError.RegisterHandler(context =>
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandardWindow("Ошибка", context.Input, ButtonEnum.Ok,
                Icon.Error, WindowStartupLocation.CenterOwner);
            messageBox.Show();
            context.SetOutput(Unit.Default);
        });

        Interactions.Error.InnerException.RegisterHandler(context =>
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandardWindow("Внутренняя ошибка", context.Input,
                ButtonEnum.Ok,
                Icon.Error, WindowStartupLocation.CenterOwner);
            messageBox.Show();
            context.SetOutput(Unit.Default);
        });

        Interactions.Notification.SuccessfulOperation.RegisterHandler(context =>
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandardWindow("Операция завершена успешно", context.Input,
                ButtonEnum.Ok,
                Icon.Success, WindowStartupLocation.CenterOwner);
            messageBox.Show();
            context.SetOutput(Unit.Default);
        });
    }

    private void ConfigureNLog()
    {
        LogManager.Setup().LoadConfiguration(builder =>
        {
            builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToConsole();
            builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToFile("logs.txt");

#if DEBUG
            builder.ForLogger().FilterMinLevel(LogLevel.Trace).WriteToConsole();
#endif
        });
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