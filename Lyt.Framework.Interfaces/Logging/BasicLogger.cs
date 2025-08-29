using System.Diagnostics;

namespace Lyt.Framework.Interfaces.Logging;

public sealed class BasicLogger : ILogger
{
    private static DateTime last = DateTime.Now;

    public bool BreakOnError { get; set; } = true;

    public void Debug(string message) => System.Diagnostics.Debug.WriteLine(MessageWithShortTimeString(message));

    public void Info(string message) => Trace.TraceInformation(MessageWithShortTimeString(message));

    public void Warning(string message) => Trace.TraceWarning(MessageWithShortTimeString(message));

    public void Error(string message)
    {
        if (this.BreakOnError && Debugger.IsAttached)
        {
            this.Debug(message);
            Debugger.Break();
        }

        Trace.TraceError(MessageWithShortTimeString(message));
    }

    public void Fatal(string message)
    {
        Trace.TraceError(MessageWithShortTimeString(message));
        if (Debugger.IsAttached) { Debugger.Break(); }
        throw new Exception(message);
    }

    public static string MessageWithShortTimeString(string message)
    {
        DateTime now = DateTime.Now;
        int deltaMs = (int)(now - last).TotalMilliseconds;
        last = now;
        return string.Format("{0}::{1} ({2}ms) - {3}", now.Second, now.Millisecond, deltaMs, message);
    }
}
