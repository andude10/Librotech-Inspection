using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Librotech_Inspection.Models;
using Librotech_Inspection.Utilities.Interactions;
using Librotech_Inspection.Utilities.Parsers.AllDataParsers.CsvFile;
using Librotech_Inspection.ViewModels.Views;
using Librotech_Inspection.Views;
using ReactiveUI;
using Splat;

namespace Librotech_Inspection.ViewModels;

public class AppBootstrapper : ReactiveObject, IScreen
{
    private Data? _data;

    public AppBootstrapper(IMutableDependencyResolver dependencyResolver = null, RoutingState testRouter = null)
    {
        Router = testRouter ?? new RoutingState();
        dependencyResolver = dependencyResolver ?? Locator.CurrentMutable;

        RegisterParts(dependencyResolver);

        Router.Navigate.Execute(new DataAnalysisViewModel(this)).Select(_ => Unit.Default);

        NavigateToDataAnalysisCommand = ReactiveCommand.CreateFromTask(NavigateToDataAnalysis);
        NavigateToLoggerConfigurationCommand = ReactiveCommand.CreateFromTask(NavigateToLoggerConfiguration);
        LoadDataCommand = ReactiveCommand.CreateFromTask(LoadData);
    }

    /// <summary>
    ///     Data that the user loads for analysis
    /// </summary>
    private Data? Data
    {
        get => _data;
        set
        {
            _data = value;
            OnDataSourceUpdated(_data);
        }
    }

#region Navigation

    public RoutingState Router { get; }

    private void RegisterParts(IMutableDependencyResolver dependencyResolver)
    {
        dependencyResolver.RegisterConstant(this, typeof(IScreen));

        dependencyResolver.Register(() => new LoggerConfigurationView(),
            typeof(IViewFor<LoggerConfigurationViewModel>));
        dependencyResolver.Register(() => new DataAnalysisView(), typeof(IViewFor<DataAnalysisViewModel>));
    }

#endregion

#region Commands

    public ReactiveCommand<Unit, Unit> NavigateToDataAnalysisCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateToLoggerConfigurationCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

#endregion

#region Methods

    private async Task NavigateToDataAnalysis()
    {
        if (Router.GetCurrentViewModel()?.GetType() != typeof(DataAnalysisViewModel))
            await Router.Navigate.Execute(new DataAnalysisViewModel(this))
                .Select(_ => Unit.Default);
    }

    private async Task NavigateToLoggerConfiguration()
    {
        if (Router.GetCurrentViewModel()?.GetType() != typeof(LoggerConfigurationViewModel))
            await Router.Navigate.Execute(new LoggerConfigurationViewModel(this))
                .Select(_ => Unit.Default);
    }

    private async Task LoadData()
    {
        var path = await DialogInteractions.ShowOpenFileDialog.Handle(Unit.Default);

        if (string.IsNullOrEmpty(path))
        {
            Debug.WriteLine("The user has not selected a file for analysis");
            return;
        }

        Data = await CsvFileParser.ParseAsync(path);
    }

#endregion

#region App events

    public delegate Task DataSourceUpdated(IReadableData? data);

    /// <summary>
    ///     OnDataSourceUpdated event occurs when the
    ///     user uploads new data, or the data changes
    /// </summary>
    public static event DataSourceUpdated OnDataSourceUpdated;

#endregion
}