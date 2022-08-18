using System;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Services;
using LibrotechInspection.Core.Services.CsvFileParser;
using LibrotechInspection.Core.Services.CsvPlotDataParser;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.Tests.TestData;
using LibrotechInspection.Desktop.Utilities.Interactions;
using LibrotechInspection.Desktop.ViewModels;
using ReactiveUI;
using Splat;
using Xunit;

namespace LibrotechInspection.Desktop.Tests.ViewModelsTests.ViewsTests;

public class MainWindowViewModelTests
{
    public MainWindowViewModelTests()
    {
        Locator.CurrentMutable.Register(() => new DebugLogger(), typeof(ILogger));
        Locator.CurrentMutable.Register(() => new CsvFileParser(), typeof(IFileRecordParser));
        Locator.CurrentMutable.Register(() => new CsvPlotDataParser(), typeof(IPlotDataParser));
        Locator.CurrentMutable.Register(() => new LinePlotCustomizer(), typeof(IPlotCustomizer));
        Locator.CurrentMutable.Register(() => new DouglasPeuckerOptimizer(), typeof(ILinePlotOptimizer));
    }

    private MainWindowViewModel BuildMainWindowViewModel()
    {
        RxApp.MainThreadScheduler = Scheduler.Immediate;
        RxApp.TaskpoolScheduler = Scheduler.Immediate;

        return new MainWindowViewModel();
    }

    [Fact]
    public void Should_go_to_DataAnalysisViewModel()
    {
        // Arrange
        var mainWindowViewModel = BuildMainWindowViewModel();
        
        // Act
        mainWindowViewModel.GoToDataAnalysisCommand.Execute().Subscribe();

        // Assert
        var currentViewModel = mainWindowViewModel.Router.GetCurrentViewModel();
        currentViewModel.Should().BeOfType<DataAnalysisViewModel>();
    }
    
    [Fact]
    public void Should_go_to_ConfigurationViewModel()
    {
        // Arrange
        var mainWindowViewModel = BuildMainWindowViewModel();
        
        // Act
        mainWindowViewModel.GoToLoggerConfigurationCommand.Execute().Subscribe();

        // Assert
        var currentViewModel = mainWindowViewModel.Router.GetCurrentViewModel();
        currentViewModel.Should().BeOfType<ConfigurationViewModel>();
    }

    [Fact]
    public async Task Should_load_valid_csv_record()
    {
        // Arrange
        var csvRecordPath = TestDataProvider.GetCsvFilePath();
        using var openFile = Interactions.Dialog.ShowOpenFileDialog
            .RegisterHandler(c => c.SetOutput(csvRecordPath));
        using var innerException = Interactions.Error.InnerException
            .RegisterHandler(c => c.SetOutput(Unit.Default));
        using var externalError =  Interactions.Error.ExternalError
            .RegisterHandler(c => c.SetOutput(Unit.Default));
        var mainWindowViewModel = BuildMainWindowViewModel();
        
        // Act
        mainWindowViewModel.GoToDataAnalysisCommand.Execute().Subscribe();
        await mainWindowViewModel.LoadRecord();

        // Assert
        mainWindowViewModel.Record.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Pass_invalid_path_should_not_load()
    {
        // Arrange
        const string csvRecordPath = "/invalid/path";
        using var openFile = Interactions.Dialog.ShowOpenFileDialog
            .RegisterHandler(c => c.SetOutput(csvRecordPath));
        using var innerException = Interactions.Error.InnerException
            .RegisterHandler(c => c.SetOutput(Unit.Default));
        using var externalError =  Interactions.Error.ExternalError
            .RegisterHandler(c => c.SetOutput(Unit.Default));
        var mainWindowViewModel = BuildMainWindowViewModel();
        
        // Act
        mainWindowViewModel.GoToDataAnalysisCommand.Execute().Subscribe();
        await mainWindowViewModel.LoadRecord();

        // Assert
        var currentViewModel = mainWindowViewModel.Router.GetCurrentViewModel();
        if (currentViewModel is not DataAnalysisViewModel dataAnalysisViewModel)
        {
            throw new Exception("ViewModel type changed unexpectedly after loading data");
        }

        dataAnalysisViewModel.PlotViewModel.PlotModel.Series.Should().BeEmpty();
    }
    
    [Fact]
    public async Task Pass_empty_path_should_not_load()
    {
        // Arrange
        using var openFile = Interactions.Dialog.ShowOpenFileDialog
            .RegisterHandler(c => c.SetOutput(string.Empty));
        using var innerException = Interactions.Error.InnerException
            .RegisterHandler(c => c.SetOutput(Unit.Default));
        using var externalError =  Interactions.Error.ExternalError
            .RegisterHandler(c => c.SetOutput(Unit.Default));
        var mainWindowViewModel = BuildMainWindowViewModel();
        
        // Act
        await mainWindowViewModel.LoadRecord();

        // Assert

        mainWindowViewModel.Record.Should().BeNull();
    }
}