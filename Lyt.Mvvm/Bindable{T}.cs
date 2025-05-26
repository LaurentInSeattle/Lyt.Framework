namespace Lyt.Mvvm;

/// <summary> Strongly typed bindable </summary>
/// <typeparam name="TControl"></typeparam>
public class Bindable<TControl> : Bindable where TControl : class, IControl, new() 
{
    public Bindable() : base() { }

    public Bindable(TControl control) : base() => this.Bind(control);

    public TControl CreateViewAndBind()
    {
        var view = new TControl();
        this.Bind(view);
        return view;
    }

    public bool IsBound => this.Control as TControl is not null ;

    public TControl View
        => this.Control as TControl ?? throw new InvalidOperationException("View is null");
}
