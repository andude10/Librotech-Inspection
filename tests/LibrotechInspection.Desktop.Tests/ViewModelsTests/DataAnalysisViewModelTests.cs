using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using LibrotechInspection.Desktop.Tests.Utilities;
using LibrotechInspection.Desktop.ViewModels;
using ReactiveUI;
using Xunit;
using Record = LibrotechInspection.Core.Models.Record.Record;

namespace LibrotechInspection.Desktop.Tests.ViewModelsTests;

public class DataAnalysisViewModelTests
{
    private DataAnalysisViewModel BuildDataAnalysisViewModel(Record? data = null)
    {
        TestSetupHelper.RegisterServices();

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