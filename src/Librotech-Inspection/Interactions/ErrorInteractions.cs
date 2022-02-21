using System;
using System.Reactive;
using ReactiveUI;

namespace Librotech_Inspection.Interactions;

public static class ErrorInteractions
{
    /// <summary>
    /// InnerException is used when an internal program error needs to be reported.
    /// Input - Exception message
    /// </summary>
    public static readonly Interaction<string, Unit> InnerException = new Interaction<string, Unit>();
    
    /// <summary>
    /// The error is used when it is necessary to notify the user of an external error
    /// (for example, incorrect data was entered or the file is being used by another process)
    /// Input - Error message
    /// </summary>
    public static readonly Interaction<string, Unit> Error = new Interaction<string, Unit>();
}