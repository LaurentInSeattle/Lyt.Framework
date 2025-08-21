namespace Lyt.Utilities.Extensions;

public static class RefectionExtensions
{
    public static bool Is<T>(this Type type) => typeof(T) == type;

    public static bool Implements<TInterface>(this Type type)
        => typeof(TInterface).IsAssignableFrom(type);

    public static bool DerivesFrom<TBase>(this Type type)
        where TBase : class
        => typeof(TBase).IsAssignableFrom(type);

    public static Action<object>? CastToActionObject<T>(this Action<T> actionOfT)
    {
        if (actionOfT == null)
        {
            return null;
        }

        return new Action<object>((o) => actionOfT((T)o));
    }

    public static void InvokeSetProperty(this object target, string propertyName, object? value)
    {
        var propertyInfo = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo is null)
        {
            return;
        }

        var methodInfo = propertyInfo.GetSetMethod();
        if (methodInfo is null)
        {
            return;
        }

        methodInfo.Invoke(target, [value]);
    }

    public static object? InvokeGetProperty(this object target, string propertyName)
    {
        var propertyInfo = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo is null)
        {
            return null;
        }

        var methodInfo = propertyInfo.GetGetMethod();
        if (methodInfo is null)
        {
            return null;
        }

        return methodInfo.Invoke(target, null);
    }
}
