using System.Reactive;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Utilities.Interactions;

public class NotificationInteractions
{
    /// <summary>
    ///     Notifies the user of a successful operation.
    ///     Input - Message
    /// </summary>
    public readonly Interaction<string, Unit> SuccessfulOperation = new();
    
    
    /// <summary>
    ///     Warns user.
    ///     Input - Message
    /// </summary>
    public readonly Interaction<string, Unit> Warn = new();
}