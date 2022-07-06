using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Models.Record;
using LibrotechInspection.Desktop.Utilities.Interactions;
using ReactiveUI;
using Splat;

namespace LibrotechInspection.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    public MainWindowViewModel()
    {
        GoToDataAnalysisCommand = ReactiveCommand.CreateFromTask(GoToDataAnalysis);
        GoToLoggerConfigurationCommand = ReactiveCommand.CreateFromTask(GoToLoggerConfiguration);
        LoadRecordCommand = ReactiveCommand.CreateFromTask(LoadRecord);

        CreateDefaultVmInstances();
        GoToDataAnalysisCommand.Execute();
    }

#region Navigation

    public RoutingState Router { get; } = new();

#endregion

#region Commands

    public ReactiveCommand<Unit, Unit> GoToDataAnalysisCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToLoggerConfigurationCommand { get; }

    public ReactiveCommand<Unit, Unit> LoadRecordCommand { get; }

#endregion

#region Methods

    private async Task GoToDataAnalysis()
    {
        await Router.Navigate.Execute(DataAnalysisViewModel.GetInstance())
            .Select(_ => Unit.Default);
    }

    private async Task GoToLoggerConfiguration()
    {
        await Router.Navigate.Execute(ConfigurationViewModel.GetInstance())
            .Select(_ => Unit.Default);
    }

    private async Task LoadRecord()
    {
        var path = await DialogInteractions.ShowOpenFileDialog.Handle(Unit.Default);

        if (string.IsNullOrEmpty(path))
        {
            Debug.WriteLine("The user has not selected a file for analysis");
            return;
        }

        var parser = Locator.Current.GetService<IFileRecordParser>();
        if (parser == null) throw new NullReferenceException();

        Record? data;
        try
        {
            data = await parser.ParseAsync(path);
        }
        catch (Exception e)
        {
            ErrorInteractions.InnerException.Handle($"Произошла внутренняя ошибка во время обработки файла. Сообщение ошибки: {e.Message}")
                .Subscribe();
            Console.WriteLine(e);
            throw;
        }

        if (data == null)
        {
            ErrorInteractions.Error.Handle("Выбран неверный формат файла, или файл используется другим процессом")
                .Subscribe();
            return;
        }

        await DataAnalysisViewModel.CreateInstanceAsync(this, data,
            Locator.Current.GetService<IPlotCustomizer>() ?? throw new InvalidOperationException());
        await ConfigurationViewModel.CreateInstanceAsync(this, data);

        // Calling navigation to update ViewModels for Views
        var currentVm = Router.GetCurrentViewModel();
        switch (currentVm)
        {
            case DataAnalysisViewModel:
                await GoToDataAnalysisCommand.Execute();
                break;
            case ConfigurationViewModel:
                await GoToLoggerConfigurationCommand.Execute();
                break;
            default:
                await GoToDataAnalysisCommand.Execute();
                break;
        }
    }

    /// <summary>
    ///     CreateDefaultVmInstances creates the first empty VMs, if the
    ///     method is not called, an error will occur during navigation
    ///     when no data is loaded.
    /// </summary>
    private void CreateDefaultVmInstances()
    {
        DataAnalysisViewModel.CreateInstanceAsync(this, null,
            Locator.Current.GetService<IPlotCustomizer>() ?? throw new InvalidOperationException());
        ConfigurationViewModel.CreateInstanceAsync(this, null);
    }

#endregion
}