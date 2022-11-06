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

public class ChartViewModelTests
{
    private ChartViewModel BuildChartViewModel(Record? data = null)
    {
        TestSetupHelper.RegisterServices();

        RxApp.MainThreadScheduler = Scheduler.Immediate;
        RxApp.TaskpoolScheduler = Scheduler.Immediate;

        return new ChartViewModel(new FixtureScreen(), data);
    }

    [Fact]
    public void Create_instance_with_no_record()
    {
        // Act
        var viewModel = BuildChartViewModel();

        // Assert
        viewModel.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_analyse_record()
    {
        // Arrange
        var record = await TestDataProvider.GetRecordOne();
        var viewModel = BuildChartViewModel(record);

        // Act
        await viewModel.StartAnalyseRecordCommand.Execute();

        // Assert
        using (new AssertionScope())
        {
            viewModel.Should().NotBeNull();
            viewModel.Record.Should().NotBeNull();
            viewModel.LinePlotViewModel.TextDataForPlot.Should().NotBeNull();
        }
    }
}