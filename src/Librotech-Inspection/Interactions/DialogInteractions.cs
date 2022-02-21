using System.Reactive;
using ReactiveUI;

namespace Librotech_Inspection.Interactions;

public static class DialogInteractions
{
    /// <summary>
    /// Show OpenFileDialog is used when a file open dialog should be displayed.
    /// Output - The path to the file
    /// </summary>
    public static readonly Interaction<Unit, string> ShowOpenFileDialog = new Interaction<Unit, string>();
}