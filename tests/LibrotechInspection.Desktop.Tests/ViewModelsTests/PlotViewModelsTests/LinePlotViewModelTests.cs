using System.Threading.Tasks;
using FluentAssertions;
using LibrotechInspection.Core.Services;
using LibrotechInspection.Core.Services.CsvPlotDataParser;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.Tests.TestData;
using LibrotechInspection.Desktop.ViewModels.PlotViewModels;
using Xunit;

namespace LibrotechInspection.Desktop.Tests.ViewModelsTests.PlotViewModelsTests;

public class LinePlotViewModelTests
{
    [Fact]
    public async Task Build_line_plot_Series_should_not_be_null()
    {
        // Arrange
        var chartData = TestDataProvider.GetPlotData();
        var vm = new LinePlotViewModel(chartData, new LinePlotCustomizer(), new CsvPlotDataParser(),
            new DouglasPeuckerOptimizer(), new PlotElementProvider());

        // Act
        await vm.BuildAsync();

        // Assert
        vm.PlotModel.Series.Should().NotBeNull();
    }
}