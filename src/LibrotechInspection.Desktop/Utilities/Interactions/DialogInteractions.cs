using System.Reactive;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Utilities.Interactions;

public static class DialogInteractions
{
    /// <summary>
    ///     ShowOpenFileDialog is used when a file open dialog should be displayed.
    ///     Output - The path to the file
    /// </summary>
    public static readonly Interaction<Unit, string?> ShowOpenFileDialog = new();
}