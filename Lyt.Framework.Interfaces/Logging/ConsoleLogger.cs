using System.Diagnostics;

namespace Lyt.Framework.Interfaces.Logging;

using static BasicLogger; 

public sealed class ConsoleLogger : ILogger
{
    public bool BreakOnError { get; set; } = true;

    public void Debug(string message) => Console.WriteLine(ShortTimeString() + message);

    public void Info(string message) => Console.WriteLine(ShortTimeString() + message);

    public void Warning(string message) => Console.WriteLine(ShortTimeString() + message);

    public void Error(string message)
    {
        if (this.BreakOnError && Debugger.IsAttached)
        {
            Debugger.Break();
        }

        Console.WriteLine(ShortTimeString() + message);
    }

    public void Fatal(string message)
    {
        Console.WriteLine(message);
        if (Debugger.IsAttached) { Debugger.Break(); }
        throw new Exception(message);
    }
}
