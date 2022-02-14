using System.Reactive;
using System.Reactive.Linq;
using Librotech_Inspection.ViewModels.MainPages;
using ReactiveUI;
using Splat;

namespace Librotech_Inspection.ViewModels;

public class MainVm : ReactiveObject, IScreen
{
    public RoutingState Router { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoNext { get; }
    public ReactiveCommand<Unit, IRoutableViewModel?> GoBack { get; }

    public MainVm()
    {
        Router = new RoutingState();
        
        GoNext = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute(new DataAnalysisVm()));
        
        var canGoBack = this
            .WhenAnyValue(x => x.Router.NavigationStack.Count)
            .Select(count => count > 0);
        GoBack = ReactiveCommand.CreateFromObservable(() => Router.NavigateBack.Execute(Unit.Default), canGoBack);
    }
}