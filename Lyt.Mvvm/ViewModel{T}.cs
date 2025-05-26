namespace Lyt.Mvvm;

/// <summary> Strongly typed view model </summary>
public class ViewModel<TView> : ViewModel where TView : class, IView, new() 
{
    public ViewModel() : base() { }

    public ViewModel(TView view) : base() => this.Bind(view);

    public TView CreateViewAndBind()
    {
        var view = new TView();
        this.Bind(view);
        return view;
    }

    public bool IsBound => this.ViewBase as TView is not null ;

    public TView View
        => this.ViewBase as TView ?? throw new InvalidOperationException("View is null");
}
