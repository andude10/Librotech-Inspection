using System.IO;
using System.Threading.Tasks;
using LibrotechInspection.Core.Services;
using LibrotechInspection.Core.Services.CsvPlotDataParser;
using LibrotechInspection.Desktop.Services;
using LibrotechInspection.Desktop.ViewModels.PlotViewModels;
using Xunit;

namespace LibrotechInspection.Desktop.Tests.ViewModelsTests.PlotViewModelsTests;

public class LinePlotViewModelTests
{
    private const string TestDataFileName = @"testChartData.csv";
    private const string TestDataDirectory = @"TestData";

    private static string GetChartData()
    {
        var path = Path.GetFullPath(Path.Combine(TestDataDirectory, TestDataFileName));

        return File.ReadAllText(path);
    }

    [Fact]
    public async Task Try_build_line_plot()
    {
        // Arrange
        var chartData = GetChartData();
        var vm = new LinePlotViewModel(new LinePlotCustomizer(), new CsvPlotDataParser(),
            new DouglasPeuckerOptimizer());

        // Act
        await vm.BuildAsync(chartData);

        // Assert
        Assert.NotNull(vm.PlotModel?.Series);
    }
}