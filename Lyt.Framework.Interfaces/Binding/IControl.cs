namespace Lyt.Framework.Interfaces.Binding;

public interface IControl
{
    object? FindControl<IControl>(string name);

    bool Focusable { get; }

    void Focus ();  
}
