using System.Diagnostics;

namespace Lyt.Framework.Interfaces.Logging;

using static BasicLogger; 

public sealed class ConsoleLogger : ILogger
{
    public bool BreakOnError { get; set; } = true;

    public void Debug(string message) => Console.WriteLine(MessageWithShortTimeString(message));

    public void Info(string message) => Console.WriteLine(MessageWithShortTimeString(message));

    public void Warning(string message) => Console.WriteLine(MessageWithShortTimeString(message));

    public void Error(string message)
    {
        if (this.BreakOnError && Debugger.IsAttached)
        {
            Debugger.Break();
        }

        Console.WriteLine(MessageWithShortTimeString(message));
    }

    public void Fatal(string message)
    {
        Console.WriteLine(MessageWithShortTimeString(message));
        if (Debugger.IsAttached) { Debugger.Break(); }
        throw new Exception(message);
    }
}
