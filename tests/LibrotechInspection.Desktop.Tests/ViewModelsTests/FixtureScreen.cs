using ReactiveUI;

namespace LibrotechInspection.Desktop.Tests.ViewModelsTests;

public class FixtureScreen : IScreen
{
    public FixtureScreen()
    {
        Router = new RoutingState();
    }

    public RoutingState Router { get; }
}