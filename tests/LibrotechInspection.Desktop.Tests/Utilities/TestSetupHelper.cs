using LibrotechInspection.Core.Interfaces;
using LibrotechInspection.Core.Services;
using LibrotechInspection.Core.Services.CsvFileParser;
using LibrotechInspection.Core.Services.CsvPlotDataParser;
using LibrotechInspection.Desktop.Services;
using Splat;

namespace LibrotechInspection.Desktop.Tests.Utilities;

public static class TestSetupHelper
{
    public static void RegisterServices()
    {
        Locator.CurrentMutable.Register(() => new DebugLogger(), typeof(ILogger));
        Locator.CurrentMutable.Register(() => new CsvFileParser(), typeof(IFileRecordParser));
        Locator.CurrentMutable.Register(() => new CsvPlotDataParser(), typeof(IPlotDataParser));
        Locator.CurrentMutable.Register(() => new LinePlotCustomizer(), typeof(IPlotCustomizer));
        Locator.CurrentMutable.Register(() => new DouglasPeuckerOptimizer(), typeof(ILinePlotOptimizer));
        Locator.CurrentMutable.Register(() => new ViewModelCache(), typeof(IViewModelCache));
    }
}