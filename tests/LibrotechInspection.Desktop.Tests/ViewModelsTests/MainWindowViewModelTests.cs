using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using FluentAssertions;
using LibrotechInspection.Desktop.Tests.Utilities;
using LibrotechInspection.Desktop.Utilities.Interactions;
using LibrotechInspection.Desktop.ViewModels;
using ReactiveUI;
using Xunit;

namespace LibrotechInspection.Desktop.Tests.ViewModelsTests;

public class MainWindowViewModelTests
{
    private MainWindowViewModel BuildMainWindowViewModel()
    {
        TestSetupHelper.RegisterServices();

        RxApp.MainThreadScheduler = Scheduler.Immediate;
        RxApp.TaskpoolScheduler = Scheduler.Immediate;

        return new MainWindowViewModel();
    }

    [Fact]
    public void Should_go_to_ChartViewModel()
    {
        // Arrange
        var mainWindowViewModel = BuildMainWindowViewModel();

        // Act
        mainWindowViewModel.GoToChartCommand.Execute().Subscribe();

        // Assert
        var currentViewModel = mainWindowViewModel.Router.GetCurrentViewModel();
        currentViewModel.Should().BeOfType<ChartViewModel>();
    }

    [Fact]
    public void Should_go_to_ConfigurationViewModel()
    {
        // Arrange
        var mainWindowViewModel = BuildMainWindowViewModel();

        // Act
        mainWindowViewModel.GoToConfigurationCommand.Execute().Subscribe();

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
        using var externalError = Interactions.Error.ExternalError
            .RegisterHandler(c => c.SetOutput(Unit.Default));
        var mainWindowViewModel = BuildMainWindowViewModel();

        // Act
        mainWindowViewModel.GoToChartCommand.Execute().Subscribe();
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
        using var externalError = Interactions.Error.ExternalError
            .RegisterHandler(c => c.SetOutput(Unit.Default));
        var mainWindowViewModel = BuildMainWindowViewModel();

        // Act
        mainWindowViewModel.GoToChartCommand.Execute().Subscribe();
        await mainWindowViewModel.LoadRecord();

        // Assert
        var currentViewModel = mainWindowViewModel.Router.GetCurrentViewModel();
        if (currentViewModel is not ChartViewModel chartViewModel)
            throw new Exception("ViewModel type changed unexpectedly after loading data");

        chartViewModel.LinePlotViewModel.ModelManager.PlotModel.Series.Should().BeEmpty();
    }

    [Fact]
    public async Task Pass_empty_path_should_not_load()
    {
        // Arrange
        using var openFile = Interactions.Dialog.ShowOpenFileDialog
            .RegisterHandler(c => c.SetOutput(string.Empty));
        using var innerException = Interactions.Error.InnerException
            .RegisterHandler(c => c.SetOutput(Unit.Default));
        using var externalError = Interactions.Error.ExternalError
            .RegisterHandler(c => c.SetOutput(Unit.Default));
        var mainWindowViewModel = BuildMainWindowViewModel();

        // Act
        await mainWindowViewModel.LoadRecord();

        // Assert

        mainWindowViewModel.Record.Should().BeNull();
    }
}