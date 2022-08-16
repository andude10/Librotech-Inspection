﻿using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Models.Record;
using LibrotechInspection.Desktop.Utilities.Interactions;
using NLog;
using NLog.Targets;
using ReactiveUI;
using Splat;

namespace LibrotechInspection.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public MainWindowViewModel()
    {
        GoToDataAnalysisCommand = ReactiveCommand.CreateFromTask(GoToDataAnalysis);
        GoToLoggerConfigurationCommand = ReactiveCommand.CreateFromTask(GoToLoggerConfiguration);
        LoadRecordCommand = ReactiveCommand.CreateFromTask(LoadRecord);
        CreateReportCommand = ReactiveCommand.CreateFromTask(CreateReport);

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
    public ReactiveCommand<Unit, Unit> CreateReportCommand { get; }

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
        Logger.Info("Start loading record");

        var recordPath = await RequestRecordPathFromUser();
        if (recordPath is null) return;

        var parser = Locator.Current.GetService<IFileRecordParser>() ??
                     throw new InvalidOperationException();

        Record? data;
        try
        {
            data = await parser.ParseAsync(recordPath);
        }
        catch (Exception e)
        {
            ErrorInteractions.InnerException
                .Handle($"Произошла внутренняя ошибка во время обработки файла. Сообщение ошибки: {e.Message}")
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

    private async Task<string?> RequestRecordPathFromUser()
    {
        var path = await DialogInteractions.ShowOpenFileDialog.Handle(Unit.Default);

        if (string.IsNullOrEmpty(path))
            Logger.Info("Record loading canceled: The user has not selected a file for analysis");

        return path;
    }

    private async Task CreateReport()
    {
        var reportFileName = await DialogInteractions.SaveTextFileDialog.Handle(Unit.Default);

        if (string.IsNullOrEmpty(reportFileName))
        {
            Logger.Info("Report creating canceled: The user has not selected a path for report");
            return;
        }

        var reportContent = new StringBuilder();
        foreach (var target in LogManager.Configuration.AllTargets)
        {
            if (target is not FileTarget fileTarget) continue;

            var logEventInfo = new LogEventInfo
            {
                TimeStamp = DateTime.Now
            };
            var fileName = fileTarget.FileName.Render(logEventInfo);

            if (!File.Exists(fileName))
                throw new Exception("Log file does not exist.");

            reportContent.Append(await File.ReadAllTextAsync(fileName));
        }

        if (string.IsNullOrEmpty(reportContent.ToString())) return;

        await File.WriteAllTextAsync(reportFileName, reportContent.ToString());
        NoticeInteractions.SuccessfulOperation.Handle($"Файл отчета успешно сохранен как '{reportFileName}'")
            .Subscribe();
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