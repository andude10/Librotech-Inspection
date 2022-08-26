using ReactiveUI;
using Splat;

namespace LibrotechInspection.Desktop.Tests.ViewModelsTests;

public class FixtureScreen : IScreen
{
    public FixtureScreen()
    {
        Locator.CurrentMutable.RegisterConstant<IScreen>(this);
        Router = new RoutingState();
    }

    public RoutingState Router { get; }
}