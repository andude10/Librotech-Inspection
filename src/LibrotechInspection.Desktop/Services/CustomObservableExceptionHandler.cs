using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using LibrotechInspection.Desktop.Utilities.Interactions;
using NLog;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Services;

public class CustomObservableExceptionHandler : IObserver<Exception>
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public void OnNext(Exception value)
    {
        if (Debugger.IsAttached) Debugger.Break();
        
        Logger.Error($"Unhandled exception occurred. \n Type: {value.GetType()}" +
                     $" \n Message: {value.Message} \n StackTrace: {value.StackTrace}");
        Interactions.Error.InnerException.Handle(
            "Произошла непредвиденная внутренняя ошибка во время работы программы. \n" +
            "Пожалуйста, отправьте отчет об ошибке разработчику ('Составить отчет о работе'). \n" +
            $"Сообщение ошибки: {value.GetType()} {value.Message}").Subscribe();

        RxApp.MainThreadScheduler.Schedule(() =>
        {
            if (Debugger.IsAttached) throw value;
        }) ;
    }

    public void OnError(Exception error)
    {
        if (Debugger.IsAttached) Debugger.Break();

        Logger.Error($"Unhandled exception occurred. \n Type: {error.GetType()}" +
                     $" \n Message: {error.Message} \n StackTrace: {error.StackTrace}");
        Interactions.Error.InnerException.Handle(
            "Произошла непредвиденная внутренняя ошибка во время работы программы. \n" +
            "Пожалуйста, отправьте отчет об ошибке разработчику ('Составить отчет о работе'). \n" +
            $"Сообщение ошибки: {error.GetType()} {error.Message}").Subscribe();

        RxApp.MainThreadScheduler.Schedule(() =>
        {
            if (Debugger.IsAttached) throw error;
        });
    }

    public void OnCompleted()
    {
        if (Debugger.IsAttached) Debugger.Break();
        RxApp.MainThreadScheduler.Schedule(() => throw new NotImplementedException());
    }
}