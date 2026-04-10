namespace Lyt.Model;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class AutoNotifyPropertyAttribute(object? initialValue = null) : Attribute
{
    /// <summary> Optional initial value for the property. Must be a compile-time constant. </summary>
    public object? InitialValue { get; } = initialValue;
}
