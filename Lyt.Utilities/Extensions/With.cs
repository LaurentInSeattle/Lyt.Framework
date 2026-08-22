namespace Lyt.Utilities.Extensions;

public static class With
{
    // Solo in class allow for cute and cool usage 
    public static void Flag(ref bool flag, Action action)
    {
        // #pragma warning disable IDE0059 
        // Unnecessary assignment of a value
        // Required by design 
        flag = true;
        try
        {
            action();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            if (Debugger.IsAttached) { Debugger.Break(); }
        }
        finally
        {
            flag = false;
        }
        // #pragma warning restore IDE0059 // Unnecessary assignment of a value
    }
}