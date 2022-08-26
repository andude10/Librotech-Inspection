using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
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
    private bool _isServicesRegistered;

    private void RegisterServices()
    {
        Locator.CurrentMutable.Register(() => new DebugLogger(), typeof(ILogger));
        Locator.CurrentMutable.Register(() => new CsvFileParser(), typeof(IFileRecordParser));
        Locator.CurrentMutable.Register(() => new CsvPlotDataParser(), typeof(IPlotDataParser));
        Locator.CurrentMutable.Register(() => new LinePlotCustomizer(), typeof(IPlotCustomizer));
        Locator.CurrentMutable.Register(() => new DouglasPeuckerOptimizer(), typeof(ILinePlotOptimizer));
        Locator.CurrentMutable.Register(() => new ViewModelCache(), typeof(IViewModelCache));
        Locator.CurrentMutable.Register(() => new PlotElementProvider(), typeof(IPlotElementProvider));
    }

    private DataAnalysisViewModel BuildDataAnalysisViewModel(Record? data = null)
    {
        if (!_isServicesRegistered)
        {
            RegisterServices();
            _isServicesRegistered = true;
        }

        RxApp.MainThreadScheduler = Scheduler.Immediate;
        RxApp.TaskpoolScheduler = Scheduler.Immediate;

        return new DataAnalysisViewModel(new FixtureScreen(), data);
    }

    [Fact]
    public void Create_instance_with_no_record()
    {
        // Act
        var viewModel = BuildDataAnalysisViewModel();

        // Assert
        viewModel.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_analyse_record()
    {
        // Arrange
        var record = await TestDataProvider.GetRecordOne();
        var viewModel = BuildDataAnalysisViewModel(record);

        // Act
        await viewModel.StartAnalyseRecordCommand.Execute();

        // Assert
        using (new AssertionScope())
        {
            viewModel.Should().NotBeNull();
            viewModel.FileShortSummary.SessionEnd.Should().NotBe(string.Empty);
            viewModel.FileShortSummary.SessionId.Should().NotBe(string.Empty);
            viewModel.FileShortSummary.SessionStart.Should().NotBe(string.Empty);
        }
    }
}