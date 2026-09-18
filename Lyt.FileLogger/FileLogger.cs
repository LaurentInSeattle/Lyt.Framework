namespace Lyt.FileLogger;

using Serilog;

public sealed class FileLogger : Lyt.Framework.Interfaces.Logging.ILogger
{
    public static string AppName = string.Empty;

    public FileLogger()
    {
        if (string.IsNullOrWhiteSpace(AppName))
        {
            throw new Exception("App Name is mising");
        }

        const string PublisherFolder = "Lyt";
        const string LogsFolder = "Logs";
        // local app data store
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string fileName = string.Format("{0}Log.txt", AppName);
        string filePath = Path.Combine(localAppData, PublisherFolder, AppName, LogsFolder, fileName);
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                filePath,
                rollingInterval: RollingInterval.Day,
                retainedFileTimeLimit: TimeSpan.FromDays(3),
                retainedFileCountLimit: 10,
                fileSizeLimitBytes: 256 * 1024 * 1024)
            .CreateLogger();

    }

    public bool BreakOnError { get => false; set { } }

    public void Debug(string message) => Log.Debug(message);

    public void Info(string message) => Log.Information(message);

    public void Warning(string message) => Log.Warning(message);

    public void Error(string message) => Log.Error(message);

    public void Fatal(string message) => Log.Fatal(message);
}
