namespace LibrotechInspection.Desktop.Services;

/// <summary>
///     Provider for interacting with local application files
/// </summary>
public interface IAppDataProvider
{
    /// <summary>
    ///     Get the path to the application local directory
    /// </summary>
    public string GetPath();

    public string GetLogsPath();
}