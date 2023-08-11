using System.IO;

namespace LibrotechInspection.Desktop.Services;

/// <summary>
///     Creates a directory for local files in the directory where the application is running.
/// </summary>
public class DefaultAppDataProvider : IAppDataProvider
{
    public const string AppDirectoryName = nameof(LibrotechInspection);

    public string GetPath()
    {
        // var dataDirectory = Path.GetTempPath();
        // var appDirectory = Path.Combine(dataDirectory, AppDirectoryName);

        Directory.CreateDirectory(AppDirectoryName);
        return AppDirectoryName;
    }
}