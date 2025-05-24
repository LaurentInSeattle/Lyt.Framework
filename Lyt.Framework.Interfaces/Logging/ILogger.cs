namespace Lyt.Framework.Interfaces.Logging;

public interface ILogger
{
    bool BreakOnError { get; set; } 
    void Debug(string message);
    void Info(string message);
    void Warning(string message);
    void Error(string message);
    void Fatal(string message);
}
