using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Librotech_Inspection.Utilities.ChartCustomizers;
using Librotech_Inspection.Utilities.Interactions;
using Librotech_Inspection.Utilities.Parsers.AllDataParsers;
using Librotech_Inspection.Utilities.Parsers.AllDataParsers.CsvFile;
using Librotech_Inspection.Utilities.Parsers.ChartDataParsers;
using Librotech_Inspection.Utilities.Parsers.ChartDataParsers.CsvFile;
using Librotech_Inspection.ViewModels.Views;
using Librotech_Inspection.Views;
using ReactiveUI;
using Splat;

namespace Librotech_Inspection.ViewModels;

public class AppBootstrapper : ReactiveObject, IScreen
{
    public AppBootstrapper(IMutableDependencyResolver dependencyResolver = null, RoutingState testRouter = null)
    {
        Router = testRouter ?? new RoutingState();
        dependencyResolver = dependencyResolver ?? Locator.CurrentMutable;

        // Service registration
        Locator.CurrentMutable.Register(() => new CsvFileParser(), typeof(DataParser));
        Locator.CurrentMutable.Register(() => new CsvChartDataParser(), typeof(ChartDataParser));
        Locator.CurrentMutable.Register(() => new LineChartCustomizer(), typeof(ChartCustomizer));
        
        RegisterParts(dependencyResolver);
        CreateDefaultVmInstances();

        NavigateToDataAnalysisCommand = ReactiveCommand.CreateFromObservable(NavigateToDataAnalysis);
        NavigateToLoggerConfigurationCommand = ReactiveCommand.CreateFromObservable(NavigateToLoggerConfiguration);
        LoadDataCommand = ReactiveCommand.CreateFromTask(LoadData);
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

    private IObservable<Unit> NavigateToDataAnalysis()
    {
        Router.Navigate.Execute(DataAnalysisViewModel.GetCurrentInstance())
            .Subscribe();

        return Observable.Return(Unit.Default);
    }

    private IObservable<Unit> NavigateToLoggerConfiguration()
    {
        Router.Navigate.Execute(LoggerConfigurationViewModel.GetCurrentInstance())
            .Subscribe();

        return Observable.Return(Unit.Default);
    }

    private async Task LoadData()
    {
        var path = await DialogInteractions.ShowOpenFileDialog.Handle(Unit.Default);

        if (string.IsNullOrEmpty(path))
        {
            Debug.WriteLine("The user has not selected a file for analysis");
            return;
        }

        var parser = Locator.Current.GetService<DataParser>();
        if (parser == null) throw new NullReferenceException();
        
        var data = await parser.ParseAsync(path);

        await DataAnalysisViewModel.CreateInstanceAsync(this, data, 
            Locator.Current.GetService<ChartCustomizer>() ?? throw new InvalidOperationException());
        await LoggerConfigurationViewModel.CreateInstanceAsync(this, data);

        NavigateToDataAnalysisCommand.Execute();
    }

    /// <summary>
    ///     CreateDefaultVmInstances creates the first empty VMs, if the
    ///     method is not called, an error will occur during navigation
    ///     when no data is loaded.
    /// </summary>
    private void CreateDefaultVmInstances()
    {
        DataAnalysisViewModel.CreateInstanceAsync(this, null, 
            Locator.Current.GetService<ChartCustomizer>() ?? throw new InvalidOperationException());
        LoggerConfigurationViewModel.CreateInstanceAsync(this, null);
    }

#endregion
}