namespace Lyt.FileLogger;

using Serilog;

public sealed class FileLogger : Lyt.Framework.Interfaces.Logging.ILogger
{
    public FileLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                "Logs/PhotoRebelLog.txt",
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
