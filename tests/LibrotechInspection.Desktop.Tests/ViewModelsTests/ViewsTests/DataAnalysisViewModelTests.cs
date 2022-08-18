using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Avalonia.Platform;
using FluentAssertions;
using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Services;
using LibrotechInspection.Core.Services.CsvFileParser;
using LibrotechInspection.Core.Services.CsvPlotDataParser;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.Tests.TestData;
using LibrotechInspection.Desktop.ViewModels;
using ReactiveUI;
using Splat;
using Xunit;
using Record = LibrotechInspection.Core.Models.Record.Record;

namespace LibrotechInspection.Desktop.Tests.ViewModelsTests.ViewsTests;

public class DataAnalysisViewModelTests
{
    public DataAnalysisViewModelTests()
    {
        Locator.CurrentMutable.Register(() => new DebugLogger(), typeof(ILogger));
        Locator.CurrentMutable.Register(() => new CsvFileParser(), typeof(IFileRecordParser));
        Locator.CurrentMutable.Register(() => new CsvPlotDataParser(), typeof(IPlotDataParser));
        Locator.CurrentMutable.Register(() => new LinePlotCustomizer(), typeof(IPlotCustomizer));
        Locator.CurrentMutable.Register(() => new DouglasPeuckerOptimizer(), typeof(ILinePlotOptimizer));
    }
    
    private async Task<DataAnalysisViewModel> BuildDataAnalysisViewModel(Record? data = null)
    {
        RxApp.MainThreadScheduler = Scheduler.Immediate;
        RxApp.TaskpoolScheduler = Scheduler.Immediate;

        await DataAnalysisViewModel.CreateInstanceAsync(new FixtureScreen(), data);
        return DataAnalysisViewModel.GetInstance();
    }
    
    [Fact]
    public async Task Create_instance_with_no_data()
    {
        // Act
        var viewModel = await BuildDataAnalysisViewModel();

        // Assert
        viewModel.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Create_instance_with_data()
    {
        // Arrange
        var record = await TestDataProvider.GetRecord();
        
        // Act
        var viewModel = await BuildDataAnalysisViewModel(record);

        // Assert
        viewModel.Should().NotBeNull();
    }
}