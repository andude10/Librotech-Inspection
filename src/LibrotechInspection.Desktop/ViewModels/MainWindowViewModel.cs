using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Models.Record;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.Utilities.Exceptions;
using LibrotechInspection.Desktop.Utilities.Interactions;
using NLog;
using NLog.Targets;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace LibrotechInspection.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly IFileRecordParser _fileRecordParser;
    private readonly IObservable<Record> _recordChange;
    private readonly ObservableAsPropertyHelper<bool> _recordHasAlarmSettings;
    private readonly ObservableAsPropertyHelper<bool> _recordHasStamps;
    private readonly ObservableAsPropertyHelper<bool> _recordIsLoaded;
    private readonly ObservableAsPropertyHelper<string> _recordName;
    private readonly IViewModelCache _viewModelCache;

    public MainWindowViewModel(IFileRecordParser? fileRecordParser = null, IViewModelCache? viewModelCache = null)
    {
        Locator.CurrentMutable.RegisterConstant<IScreen>(this);

        _fileRecordParser = fileRecordParser ?? Locator.Current.GetService<IFileRecordParser>()
            ?? throw new NoServiceFound(nameof(IFileRecordParser));
        _viewModelCache = viewModelCache ?? Locator.Current.GetService<IViewModelCache>()
            ?? throw new NoServiceFound(nameof(IViewModelCache));

        Router = new RoutingState();
        GoToChartCommand = ReactiveCommand.CreateFromTask(GoToChart);
        GoToDeviceAlarmSettingsCommand = ReactiveCommand.CreateFromTask(GoToDeviceAlarmSettings);
        GoToStampsCommand = ReactiveCommand.CreateFromTask(GoToStamps);
        LoadRecordCommand = ReactiveCommand.CreateFromTask(LoadRecord);
        CreateReportCommand = ReactiveCommand.CreateFromTask(CreateReport);

        _recordChange = this.WhenAnyValue(vm => vm.Record)
            .WhereNotNull();

        _recordIsLoaded = this.WhenAnyValue(vm => vm.Record)
            .Select(record => record is not null)
            .ToProperty(this, x => x.RecordIsLoaded);

        _recordHasStamps = _recordChange.Select(record => record.Stamps is not null)
            .ToProperty(this, x => x.RecordHasStamps);

        _recordHasAlarmSettings = _recordChange.Select(record => record.DeviceAlarmSettings is not null)
            .ToProperty(this, x => x.RecordHasAlarmSettings);

        _recordName = _recordChange.Select(record => $"{nameof(LibrotechInspection)} - {record.Name}")
            .ToProperty(this, x => x.WindowTitle);

        GoToChartCommand.Execute();
    }

    #region Properties

    public RoutingState Router { get; }
    [Reactive] public Record? Record { get; private set; }
    public bool RecordHasStamps => _recordHasStamps.Value;
    public bool RecordHasAlarmSettings => _recordHasAlarmSettings.Value;
    public bool RecordIsLoaded => _recordIsLoaded.Value;
    public string WindowTitle => _recordName.Value;

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> GoToChartCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToDeviceAlarmSettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToStampsCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadRecordCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateReportCommand { get; }

    #endregion

    #region Methods

    private async Task GoToChart()
    {
        var viewModel = (ChartViewModel)await _viewModelCache.GetOrCreate(typeof(ChartViewModel),
            () => new ChartViewModel(this, Record));

        await SavePreviousViewModelToCache();

        await Router.Navigate.Execute(viewModel)
            .Select(_ => Unit.Default);
    }

    private async Task GoToDeviceAlarmSettings()
    {
        if (Record?.DeviceSpecifications is null) return;

        var viewModel = (DeviceAlarmSettingsViewModel)await _viewModelCache.GetOrCreate(
            typeof(DeviceAlarmSettingsViewModel),
            () => new DeviceAlarmSettingsViewModel(Record));

        await SavePreviousViewModelToCache();

        await Router.Navigate.Execute(viewModel)
            .Select(_ => Unit.Default);
    }

    private async Task GoToStamps()
    {
        if (Record?.Stamps is null) return;

        var viewModel = (StampsViewModel)await _viewModelCache.GetOrCreate(typeof(StampsViewModel),
            () => new StampsViewModel(Record));

        await SavePreviousViewModelToCache();

        await Router.Navigate.Execute(viewModel)
            .Select(_ => Unit.Default);
    }

    public async Task LoadRecord()
    {
        Logger.Info("Start loading record");

        var recordPath = await RequestRecordPathFromUser();
        if (recordPath is null) return;

        try
        {
            Record = await _fileRecordParser.ParseAsync(recordPath);
        }
        catch (Exception e)
        {
            Interactions.Error.InnerException
                .Handle($"Произошла внутренняя ошибка во время обработки файла. Сообщение ошибки: {e.Message}")
                .Subscribe();
            throw;
        }

        if (Record == null)
        {
            Interactions.Error.ExternalError
                .Handle("Выбран неверный формат файла, или файл используется другим процессом")
                .Subscribe();
            return;
        }

        await CreateNewViewModelsCache();
    }

    // TODO: Change the cache system (Or navigation) so that it is unnecessary to update the cache when a new record is loaded
    private async Task CreateNewViewModelsCache()
    {
        if (Record is null)
        {
            const string message = "Start creating viewModel cache, although Record is null";
            Logger.Error(message);
            throw new InvalidOperationException(message);
        }

        var chartViewModel = new ChartViewModel(this, Record);
        await chartViewModel.StartAnalyseRecordCommand.Execute();

        await _viewModelCache.Save(chartViewModel);

        if (RecordHasStamps)
            await _viewModelCache.Save(new StampsViewModel(Record));
        if (RecordHasAlarmSettings)
            await _viewModelCache.Save(new DeviceAlarmSettingsViewModel(Record));

        await NavigateToCurrentViewModel();
    }

    private async Task SavePreviousViewModelToCache()
    {
        var viewModel = Router.GetCurrentViewModel();

        if (viewModel is not ViewModelBase viewModelBase) return;

        await _viewModelCache.Save(viewModelBase);
    }

    private async Task NavigateToCurrentViewModel()
    {
        var currentVm = Router.GetCurrentViewModel();
        switch (currentVm)
        {
            case ChartViewModel:
                await GoToChartCommand.Execute();
                break;
            case DeviceAlarmSettingsViewModel:
                await GoToDeviceAlarmSettingsCommand.Execute();
                break;
            case StampsViewModel:
                await GoToStampsCommand.Execute();
                break;
            default:
                await GoToChartCommand.Execute();
                break;
        }
    }

    private async Task<string?> RequestRecordPathFromUser()
    {
        var path = await Interactions.Dialog.ShowOpenFileDialog.Handle(Unit.Default);

        if (string.IsNullOrEmpty(path))
            Logger.Info("Record loading canceled: The user has not selected a file for analysis");

        return path;
    }

    private async Task CreateReport()
    {
        var reportFileName = await Interactions.Dialog.SaveTextFileDialog.Handle(Unit.Default);

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
        Interactions.Notification.SuccessfulOperation.Handle($"Файл отчета успешно сохранен как '{reportFileName}'")
            .Subscribe();
    }

    #endregion
}