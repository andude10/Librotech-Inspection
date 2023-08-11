using System.IO;

namespace LibrotechInspection.Desktop.Services;

/// <summary>
///     Creates a directory for local files in the directory where the application is running.
/// </summary>
public class DefaultAppDataProvider : IAppDataProvider
{
    public const string AppDirectoryName = nameof(LibrotechInspection);
    public const string LogsFileName = "logs.txt";

    public string GetPath()
    {
        var tempPath = Path.GetTempPath();
        var appDirectory = Path.Combine(tempPath, AppDirectoryName);

        Directory.CreateDirectory(appDirectory);
        return appDirectory;
    }

    public string GetLogsPath()
    {
        var appDirectory = GetPath();
        var logsPath = Path.Combine(appDirectory, LogsFileName);

        return logsPath;
    }
}