// #define VERBOSE_Bindable

namespace Lyt.Mvvm;

/// <summary> Bindable class, aka a View Model.  </summary>
public class ViewModel : ObservableObject, ISupportBehaviors, IBindable
{
    private static ILocalizer? StaticLocalizer;

#pragma warning disable CS8618 
    // Non-nullable field must contain a non-null value when exiting constructor.
    // Consider adding the 'required' modifier or declaring as nullable.

    private static IHost StaticHost;

    private static ILogger StaticLogger;

    private static IMessenger StaticMessenger;

    private static IProfiler StaticProfiler;

#pragma warning restore CS8618 

    public static void TypeInitialize(IHost host)
    {
        StaticHost = host;
        var services = host.Services;
        if (services is null)
        {
            Debug.WriteLine("No services \n");
            throw new Exception("No services.");
        }

        try
        {
            StaticMessenger = host.Services.GetRequiredService<IMessenger>();
            StaticLogger = host.Services.GetRequiredService<ILogger>();
            StaticProfiler = host.Services.GetRequiredService<IProfiler>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Missing essential services \n" + ex.ToString());
            throw;
        }

        try
        {
            StaticLocalizer = services.GetService<ILocalizer>();
            if (StaticLocalizer is null)
            {
                StaticLogger.Error("Missing localizer service, will not be able to localize. \n");
            }
        }
        catch (Exception ex)
        {
            StaticLogger.Error("Exception when trying to retrieve localizer service. \n" + ex.ToString());
        }
    }

    public ViewModel() {    }

#pragma warning disable IDE0079
#pragma warning disable CA1822 // Mark members as static

    public IHost Host => StaticHost;

    public ILocalizer Localizer =>
        this.CanLocalize ? StaticLocalizer! : throw new Exception("Should have checked CanLocalize property.");

    public IMessenger Messenger => StaticMessenger;

    public IProfiler Profiler => StaticProfiler;

#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079

    /// <summary> The control, its Data Context is this instance. </summary>
    /// <remarks> Aka, the "View" </remarks>
    public IView? ViewBase { get; private set; }

    public List<object> Behaviors { get; private set; } = [];

    /// <summary> Binds a control and setup callbacks. </summary>
    public void BindOnDataContextChanged(IView view)
    {
        this.ViewBase = view;
        this.OnDataBinding();
    }

    /// <summary> Binds a control and setup callbacks. </summary>
    public void Bind(IView view)
    {
        this.ViewBase = view;
        this.Unbind();
        try
        {
            this.ViewBase.DataContext = this;
        }
        catch (InvalidCastException ex)
        {
            // Crash here ? This should never happen, ever. 
            // Major issue when defining the view, usually conflicting DaataContext by inheritance
            if (Debugger.IsAttached) { Debugger.Break(); }
            Debug.WriteLine(this.GetType().FullName);
            Debug.WriteLine(this.ViewBase.GetType().FullName);
            Debug.WriteLine(this.ViewBase.DataContext?.GetType().FullName);
            Debug.WriteLine(ex);
        }

        this.OnDataBinding();
    }

    /// <summary> Unbinds this bindable. </summary>
    public void Unbind()
    {
        if (this.ViewBase is not null)
        {
            if (this.ViewBase.DataContext != null)
            {
                this.ViewBase.DataContext = null;
            }
        }
    }

    /// <summary> Unbinds the provided control. </summary>
    public static void Unbind(IView view)
    {
        if (view is not null)
        {
            if (view.DataContext is ViewModel bindable)
            {
                bindable.ViewBase = null;
                view.DataContext = null;
            }
        }
    }

    /// <summary> Invoked when this bindable is bound </summary>
    protected virtual void OnDataBinding() { }

    /// <summary> Invoked when this bindable control is loaded. </summary>
    public virtual void OnViewLoaded() { }

    /// <summary> Usually invoked when this bindable is about to be shown, but could be used for other purposes. </summary>
    public virtual void Activate(object? activationParameters) => this.LogActivation(activationParameters);

    /// <summary> Usually invoked when this bindable is about to be hidden, and same as above. </summary>
    public virtual void Deactivate() => this.LogDeactivation();

    public virtual bool CanEscape { get; set; } = true;

    public virtual bool CanEnter { get; set; } = true;

    public virtual bool Validate() => true;

    public virtual bool TrySaveAndClose() => true;

    public virtual void CancelViewModel() { }

    public virtual void Cancel() { }

    #region IBindable implementation 

    public void Set(string message, string messagePropertyName)
        //    => _ = this.Set<string>(message, messagePropertyName);
        => this.InvokeSetProperty(messagePropertyName, message);

    public string? Get(string sourcePropertyName)
    {
        // => this.Get<string>(sourcePropertyName);
        object? maybeString = this.InvokeGetProperty(sourcePropertyName);
        if ( maybeString is string realString)
        {
            return realString; 
        }

        return null;
    } 

    public ILogger Logger => StaticLogger;

    public bool CanLocalize => StaticLocalizer is not null;

    public string Localize(string message)
    {
        if (this.CanLocalize)
        {
            return this.Localize(message);
        }

        return message;
    }

    public bool TryFocus(string focusFieldName)
    {
        //var field = this.GetControlByName(focusFieldName);
        //if (field is IControl control && control.Focusable)
        //{
        //    // viewModel.Logger.Debug(viewModel.GetType().Name + ": Focus on : " + focusFieldName);
        //    // Why we need to wait is still a bit of a mistery !
        //    // Schedule.OnUiThread(222, () => { control.Focus(); }, DispatcherPriority.ApplicationIdle);
        //    return true;
        //}

        return false;
    }

    public IView? GetControlByName(string name)
    {
        //if ((string.IsNullOrWhiteSpace(name)) || (this.Control is null))
        //{
        //    return null;
        //}

        //object? maybeControl = this.Control.FindControl<Control>(name);
        //if (maybeControl is Control control)
        //{
        //    return control;
        //}

        return null;
    }

    #endregion IBindable implementation 

    #region Debug Utilities 

    /// <summary> Logs that a bindable is being deactivated. </summary>
    [Conditional("DEBUG")]
    private void LogDeactivation()
    {
        string typeName = this.GetType().Name;
        string message = string.Format("Deactivating {0}", typeName);
        this.Logger.Info(message);
    }

    /// <summary> Logs that a bindable is being activated. </summary>
    [Conditional("DEBUG")]
    private void LogActivation(object? parameter)
    {
        string parameterString =
            parameter is null ? "<null>" : parameter.GetType().Name + " - " + parameter.ToString();
        string typeName = this.GetType().Name;
        string message = string.Format("Activating {0} with {1}", typeName, parameterString);
        this.Logger.Info(message);
    }


    #endregion Debug Utilities 
}
