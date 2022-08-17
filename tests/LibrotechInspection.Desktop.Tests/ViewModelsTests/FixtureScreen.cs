using ReactiveUI;

namespace LibrotechInspection.Desktop.Tests.ViewModelsTests;

public class TestScreen : IScreen
{
    public TestScreen()
    {
        Router = new RoutingState();
    }

    public RoutingState Router { get; }
}