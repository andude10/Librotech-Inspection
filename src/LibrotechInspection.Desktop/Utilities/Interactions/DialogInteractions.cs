using System.Reactive;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace LibrotechInspection.Desktop.Utilities.Interactions;

public static class DialogInteractions
{
    /// <summary>
    ///     ShowOpenFileDialog is used when a file open dialog should be displayed.
    ///     Output - The path to the file
    /// </summary>
    public static readonly Interaction<Unit, string?> ShowOpenFileDialog = new();

    /// <summary>
    ///     ShowSaveFileDialog is used when a save file dialog should be displayed.
    ///     Output - The path to the created text file
    /// </summary>
    public static readonly Interaction<Unit, string?> SaveTextFileDialog = new();

    /// <summary>
    ///     Save IBitmap as a file with the path specified by the user
    ///     Input - IBitmap image to save, initial file name
    /// </summary>
    public static readonly Interaction<(IBitmap, string), Unit> SaveBitmapAsPng = new();
}