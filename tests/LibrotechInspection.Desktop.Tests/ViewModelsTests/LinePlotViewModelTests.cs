using System.Threading.Tasks;
using FluentAssertions;
using LibrotechInspection.Desktop.Tests.Utilities;
using LibrotechInspection.Desktop.ViewModels.PlotViewModels;
using Xunit;

namespace LibrotechInspection.Desktop.Tests.ViewModelsTests;

public class LinePlotViewModelTests
{
    [Fact]
    public async Task Build_line_plot_Series_should_not_be_null()
    {
        // Arrange
        var chartData = TestDataProvider.GetPlotData();
        TestSetupHelper.RegisterServices();
        var vm = new LinePlotViewModel(chartData);

        // Act
        await vm.BuildAsync();

        // Assert
        vm.ModelManager.PlotModel.Series.Should().NotBeNull();
    }
}