using System.Reactive;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Utilities.Interactions;

public static class NoticeInteractions
{
    /// <summary>
    ///     Notifies the user of a successful operation.
    ///     Input - Message
    /// </summary>
    public static readonly Interaction<string, Unit> SuccessfulOperation = new();
}