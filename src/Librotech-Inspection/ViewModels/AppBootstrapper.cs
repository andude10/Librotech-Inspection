﻿using Librotech_Inspection.Views;
using ReactiveUI;
using Splat;

namespace Librotech_Inspection.ViewModels;
/* COOLSTUFF: What is the AppBootstrapper?
 * 
 * The AppBootstrapper is like a ViewModel for the WPF Application class.
 * Since Application isn't very testable (just like Window / UserControl), 
 * we want to create a class we can test. Since our application only has
 * one "screen" (i.e. a place we present Routed Views), we can also use 
 * this as our IScreen.
 * 
 * An IScreen is a ViewModel that contains a Router - practically speaking,
 * it usually represents a Window (or the RootFrame of a WinRT app). We 
 * should technically create a MainWindowViewModel to represent the IScreen,
 * but there isn't much benefit to split those up unless you've got multiple
 * windows.
 * 
 * AppBootstrapper is a good place to implement a lot of the "global 
 * variable" type things in your application. It's also the place where
 * you should configure your IoC container. And finally, it's the place 
 * which decides which View to Navigate to when the application starts.
 */

public class AppBootstrapper : ReactiveObject, IScreen
{
    public AppBootstrapper(IMutableDependencyResolver dependencyResolver = null, RoutingState testRouter = null)
    {
        Router = testRouter ?? new RoutingState();
        dependencyResolver = dependencyResolver ?? Locator.CurrentMutable;

        // Bind 
        RegisterParts(dependencyResolver);

        // TODO: This is a good place to set up any other app startup tasks

        // Navigate to the opening page of the application
        Router.Navigate.Execute(new WelcomeViewModel(this));
    }

    public RoutingState Router { get; }

    private void RegisterParts(IMutableDependencyResolver dependencyResolver)
    {
        dependencyResolver.RegisterConstant(this, typeof(IScreen));

        dependencyResolver.Register(() => new WelcomeView(), typeof(IViewFor<WelcomeViewModel>));
        dependencyResolver.Register(() => new DataAnalysisView(), typeof(IViewFor<DataAnalysisViewModel>));
    }
}