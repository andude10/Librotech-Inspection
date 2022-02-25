using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace Librotech_Inspection.ViewModels.Views;

public class WelcomeViewModel : ReactiveObject, IRoutableViewModel
{
    /* COOLSTUFF: Why the Screen here?
     *
     * Every RoutableViewModel has a pointer to its IScreen. This is really
     * useful in a unit test runner, because you can create a dummy screen,
     * invoke Commands / change Properties, then test to see if you navigated
     * to the correct new screen 
     */
    public WelcomeViewModel(IScreen screen)
    {
        HostScreen = screen;

        NavigateToSecond = ReactiveCommand.CreateFromTask(async () =>
            await HostScreen.Router.Navigate.Execute(new DataAnalysisViewModel(HostScreen)).Select(_ => Unit.Default));

        this.WhenNavigatedTo(() => Bar());
    }

    public ReactiveCommand<Unit, Unit> NavigateToSecond { get; }

    /* COOLSTUFF: What is UrlPathSegment
     * 
     * Imagine that the router state is like the path of the URL - what 
     * would the path look like for this particular page? Maybe it would be
     * the current user's name, or an "id". In this case, it's just a 
     * constant. You can get the whole path via 
     * IRoutingState.GetUrlForCurrentRoute.
     */
    public string UrlPathSegment => "welcome";

    public IScreen HostScreen { get; protected set; }

    private IDisposable Bar()
    {
        return Disposable.Create(() => Foo());
    }

    private void Foo()
    {
        if (true)
        {
        }
    }
}