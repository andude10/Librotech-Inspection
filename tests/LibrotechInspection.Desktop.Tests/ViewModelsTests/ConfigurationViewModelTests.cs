using System.Reactive.Concurrency;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using LibrotechInspection.Desktop.Tests.Utilities;
using LibrotechInspection.Desktop.ViewModels;
using ReactiveUI;
using Xunit;
using Record = LibrotechInspection.Core.Models.Record.Record;

namespace LibrotechInspection.Desktop.Tests.ViewModelsTests;

public class ConfigurationViewModelTests
{
    private static ConfigurationViewModel BuildConfigurationViewModel(Record? data = null)
    {
        RxApp.MainThreadScheduler = Scheduler.Immediate;
        RxApp.TaskpoolScheduler = Scheduler.Immediate;

        var viewModel = new ConfigurationViewModel(new FixtureScreen(), data);
        return viewModel;
    }

    [Fact]
    public async Task Create_instance_should_load_data()
    {
        // Arrange
        var record = await TestDataProvider.GetRecordOne();

        // Act
        var viewModel = BuildConfigurationViewModel(record);

        // Assert
        using (new AssertionScope())
        {
            viewModel.Should().NotBeNull();
            viewModel.Stamps.Should().NotBeEmpty();
            viewModel.DeviceSpecifications.Should().NotBeEmpty();
            viewModel.EmergencyEventsSettings.Should().NotBeEmpty();
        }
    }
}