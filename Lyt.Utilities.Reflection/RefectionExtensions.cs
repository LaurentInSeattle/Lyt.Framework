namespace Lyt.Utilities.Reflection;

public static class RefectionExtensions
{
    public static bool TryParseAny<T>(this string input, out T? result)
    {
        result = default;
        Type type = typeof(T);
        bool isIParsableOfT =
            typeof(T)
            .GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IParsable<>));
        if (!isIParsableOfT)
        {
            return false;
        }

        var parsableInterface =
            typeof(T)
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IParsable<>));
        if (parsableInterface is null)
        {
            return false;
        }

        var parseMethod = parsableInterface.GetMethod("TryParse", [typeof(string), type.MakeByRefType()]);
        if (parseMethod is null)
        {
            return false;
        }

        object?[] parameters = [input, CultureInfo.InvariantCulture, null];
        object? maybeBool = parseMethod.Invoke(null, parameters);
        if (maybeBool is not bool isParsed)
        {
            return false;
        }

        if (isParsed)
        {
            object? resultObject = parameters[2];
            if (resultObject is T typedResult)
            {
                result = typedResult;
                return true;
            }
        }

        return false;
    }

    public static bool TryGetPropertyType(
        this object target, string propertyName, [NotNullWhen(true)] out Type? propertyType)
    {
        propertyType = null;
        var propertyInfo = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo is null)
        {
            return false;
        }

        var methodInfo = propertyInfo.GetGetMethod();
        if (methodInfo is null)
        {
            return false;
        }

        propertyType = methodInfo.ReturnType;
        return true;
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
