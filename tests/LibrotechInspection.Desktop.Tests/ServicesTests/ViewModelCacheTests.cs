using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using LibrotechInspection.Core.Models.Record;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.Tests.Utilities;
using LibrotechInspection.Desktop.ViewModels;
using LibrotechInspection.Desktop.ViewModels.PlotViewModels;
using Xunit;

namespace LibrotechInspection.Desktop.Tests.ServicesTests;

public class ViewModelCacheTests
{
    [Fact]
    public async Task Should_create_ChartViewModel_cache()
    {
        // Arrange
        TestSetupHelper.RegisterServices();
        var viewModelCache = new ViewModelCache();

        // Act
        var viewModel = await viewModelCache.GetOrCreate(typeof(ChartViewModel),
            () => new ChartViewModel(new FixtureScreen()));

        // Assert
        viewModel.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_create_ChartViewModel_with_fixture_record_and_return_from_cache_with_same_record()
    {
        // Arrange
        TestSetupHelper.RegisterServices();
        var viewModelCache = new ViewModelCache();
        var screen = new FixtureScreen();
        var record = new FileRecord { PlotData = "test123" };

        // Act
        var createdViewModel = (ChartViewModel)await viewModelCache.GetOrCreate(typeof(ChartViewModel),
            () => new ChartViewModel(screen, record));
        var fromCacheViewModel = (ChartViewModel)await viewModelCache.GetOrCreate(typeof(ChartViewModel),
            () => new ChartViewModel(new FixtureScreen()));

        // Assert
        using (new AssertionScope())
        {
            fromCacheViewModel.Should().BeAssignableTo<ChartViewModel>();
            fromCacheViewModel.Record.Should().BeEquivalentTo(createdViewModel.Record);
        }
    }

    [Fact]
    public async Task Should_save_ChartViewModel_with_fixture_record_to_cache_and_return_with_same_record()
    {
        // Arrange
        TestSetupHelper.RegisterServices();
        var viewModelCache = new ViewModelCache();
        var screen = new FixtureScreen();
        var record = new FileRecord { PlotData = "test123" };
        var createdViewModel = new ChartViewModel(screen, record);

        // Act
        await viewModelCache.Save(createdViewModel);
        var fromCacheViewModel = (ChartViewModel)await viewModelCache.GetOrCreate(typeof(ChartViewModel),
            () => new ChartViewModel(new FixtureScreen()));

        // Assert
        using (new AssertionScope())
        {
            fromCacheViewModel.Should().BeAssignableTo<ChartViewModel>();
            fromCacheViewModel.Record.Should().BeEquivalentTo(createdViewModel.Record);
        }
    }

    [Fact]
    public async Task Should_create_ChartViewModel_with_recordOne_and_return_with_same_PlotViewModel()
    {
        // Arrange
        TestSetupHelper.RegisterServices();
        var viewModelCache = new ViewModelCache();
        var screen = new FixtureScreen();
        var record = await TestDataProvider.GetRecordOne();

        // Act
        var createdViewModel = (ChartViewModel)await viewModelCache.GetOrCreate(typeof(ChartViewModel),
            () => new ChartViewModel(screen, record));
        await createdViewModel.StartAnalyseRecordCommand.Execute();
        await viewModelCache.Save(createdViewModel);

        var fromCacheViewModel = (ChartViewModel)await viewModelCache.GetOrCreate(typeof(ChartViewModel),
            () => new ChartViewModel(new FixtureScreen()));

        // Assert
        using (new AssertionScope())
        {
            fromCacheViewModel.Should().BeAssignableTo<ChartViewModel>();
            fromCacheViewModel.LinePlotViewModel.TextDataForPlot.Should()
                .BeEquivalentTo(createdViewModel.LinePlotViewModel.TextDataForPlot);
            fromCacheViewModel.LinePlotViewModel.DisplayConditions.Should()
                .BeEquivalentTo(createdViewModel.LinePlotViewModel.DisplayConditions);
            fromCacheViewModel.LinePlotViewModel.PlotType.Should()
                .BeEquivalentTo(createdViewModel.LinePlotViewModel.PlotType);
        }
    }

    [Fact]
    public async Task Should_create_ChartViewModel_with_recordOne_and_return_with_same_LinePlotViewModel()
    {
        // Arrange
        TestSetupHelper.RegisterServices();
        var viewModelCache = new ViewModelCache();
        var screen = new FixtureScreen();
        var record = await TestDataProvider.GetRecordOne();
        var linePlotViewModel = new LinePlotViewModel();

        // Act
        var createdViewModel = (ChartViewModel)await viewModelCache.GetOrCreate(typeof(ChartViewModel),
            () => new ChartViewModel(screen, record, linePlotViewModel));
        await createdViewModel.StartAnalyseRecordCommand.Execute();
        await viewModelCache.Save(createdViewModel);

        var fromCacheViewModel = (ChartViewModel)await viewModelCache.GetOrCreate(typeof(ChartViewModel),
            () => new ChartViewModel(new FixtureScreen()));

        // Assert
        using (new AssertionScope())
        {
            fromCacheViewModel.Should().BeAssignableTo<ChartViewModel>();
            fromCacheViewModel.LinePlotViewModel.Should().BeAssignableTo<LinePlotViewModel>();

            var fromCachePlotModel = (LinePlotViewModel)fromCacheViewModel.LinePlotViewModel;
            var originalPlotModel = (LinePlotViewModel)createdViewModel.LinePlotViewModel;

            var cachePlotModelSeriesCount = fromCachePlotModel.ModelManager.PlotModel.Series.Count;
            var originalPlotModelSeriesCount = originalPlotModel.ModelManager.PlotModel.Series.Count;
            cachePlotModelSeriesCount.Should().Be(originalPlotModelSeriesCount);

            fromCachePlotModel.DisplayConditions.Should().BeEquivalentTo(originalPlotModel.DisplayConditions);
        }
    }

    [Fact]
    public async Task Should_save_DeviceAlarmSettings_with_fixture_record_and_return_with_same_record()
    {
        // Arrange
        TestSetupHelper.RegisterServices();
        var viewModelCache = new ViewModelCache();
        var screen = new FixtureScreen();
        var record = new FileRecord { PlotData = "test123" };
        var createdViewModel = new DeviceAlarmSettingsViewModel(record, screen);

        // Act
        await viewModelCache.Save(createdViewModel);
        var fromCacheViewModel = (DeviceAlarmSettingsViewModel)await viewModelCache.GetOrCreate(
            typeof(DeviceAlarmSettingsViewModel),
            () => new DeviceAlarmSettingsViewModel(record, new FixtureScreen()));

        // Assert
        using (new AssertionScope())
        {
            fromCacheViewModel.Should().BeAssignableTo<DeviceAlarmSettingsViewModel>();
            fromCacheViewModel.Record.Should().BeEquivalentTo(createdViewModel.Record);
        }
    }
}