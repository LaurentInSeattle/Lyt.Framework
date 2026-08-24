namespace Lyt.Model;

[AttributeUsage(AttributeTargets.Property)]
public class ModelDoNotLogAttribute : Attribute { }

public abstract class ModelBase(ILogger logger) : IModel
{
    private bool isDirty;

    public abstract Task Initialize();

    public virtual Task Configure(object? modelConfiguration) => Task.CompletedTask;

    public virtual Task Shutdown()
    {
        this.Clear();
        return Task.CompletedTask;
    }

    public virtual Task Save()
    {
        this.IsDirty = false;
        return Task.CompletedTask;
    }

    [JsonIgnore]
    public ILogger Logger { get; private set; } = logger;

    [JsonIgnore]
    public bool IsDirty
    {
        get => this.isDirty;
        protected set
        {
            if ( this.IsInitializing)
            {
                return;
            }

            this.isDirty = value;
            if (value)
            {
                this.Save();
            }
        }
    }

    [JsonIgnore]
    public bool ShouldAutoSave { get; protected set; }

    [JsonIgnore]
    public bool IsInitializing { get; set; }

    /// <summary> Allows to disable logging when properties are changing so that we do not flood the logs. </summary>
    /// <remarks> Use for quickly changing properties, mouse, sliders, etc.</remarks>
    [JsonIgnore]
    public bool DisablePropertyChangedLogging { get; protected set; }

    /// <summary> The model properties.</summary>
    protected readonly Dictionary<string, object?> properties = [];

    public void Clean() => this.IsDirty = false;

    protected void NotifyUpdate(string propertyName = "", string methodName = "")
        => new ModelUpdateMessage(this, propertyName, methodName).Publish();

    /// <summary> Gets the value of a property </summary>
    protected T? Get<T>([CallerMemberName] string? name = null)
    {
        if (name is null)
        {
            this.Logger.Error("Get property: no name");
            throw new Exception("Get property: no name");
        }

        return this.properties.TryGetValue(name, out object? value) ? value == null ? default : (T)value : default;
    }

    /// <summary> Sets the value of a property, AND changes the dirty state of the model  </summary>
    /// <returns> True, if the value was changed, false otherwise. </returns>
    protected bool Set<T>(T? value, [CallerMemberName] string? name = null)
    {
        if (name is null)
        {
            this.Logger.Error("Set property: no name");
            throw new Exception("Set property: no name");
        }

        if (Equals(value, this.Get<T>(name)))
        {
            return false;
        }

        return this.PrivateSet<T>(value, name, setDirty: true);
    }

    /// <summary> Sets the value of a property, without changing the dirty state of the model  </summary>
    /// <returns> True, if the value was changed, false otherwise. </returns>
    protected bool SetClean<T>(T? value, [CallerMemberName] string? name = null)
    {
        if (name is null)
        {
            this.Logger.Error("Set property: no name");
            throw new Exception("Set property: no name");
        }

        if (Equals(value, this.Get<T>(name)))
        {
            return false;
        }

        return this.PrivateSet<T>(value, name, setDirty: false);
    }

    private bool PrivateSet<T>(T? value, string name, bool setDirty)
    {
        this.properties[name] = value;
        this.NotifyUpdate(name);
        if (setDirty)
        {
            this.IsDirty = true;
        } 

        if (!this.DisablePropertyChangedLogging)
        {
            // Conditional debug 
            this.LogPropertyChanged(name, value);
        }

        return true;
    }

    /// <summary> Clear and Dispose when applicable, all properties </summary>
    protected void Clear()
    {
        foreach (object? property in this.properties.Values)
        {
            if (property is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        this.properties.Clear();
    }

    //[RequiresUnreferencedCode("Not trimming safe")]
    protected void CopyJSonRequiredProperties<T>(T source)
    {
        if (source is not ModelBase)
        {
            throw new Exception("Source is not a model.");
        }
#pragma warning disable IL2075
        // Apparently just works fine 
        var allProperties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
#pragma warning restore IL2075

        List<PropertyInfo> copyProperties = new (allProperties.Length);
        for (int i = 0; i < allProperties.Length; ++i)
        {
            var property = allProperties[i];
            object[] attributes = property.GetCustomAttributes(typeof(JsonRequiredAttribute), true);
            if (attributes.Length > 0)
            {
                copyProperties.Add(property);
            }
        }

        foreach (PropertyInfo property in copyProperties)
        {
            object? value = property.GetValue(source, null);
            property.SetValue(this, value, null);
        }
    }

    #region Debug Utilities 

    /// <summary> Logs that a model property is changing. </summary>
    [Conditional("DEBUG")]
    private void LogPropertyChanged(string name, object? value)
    {
        if (this.Logger is null)
        {
            return;
        }

        string message =
            string.Format(" Property {0} changed to:   {1}", name, value == null ? "null" : value.ToString());
        this.Logger.Info(message);
    }

    #endregion Debug Utilities 
}
