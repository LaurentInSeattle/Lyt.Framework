namespace Lyt.Framework.Interfaces.Binding; 

public interface IView
{
    object? DataContext { get; set; }

    bool IsVisible { get; set; }
}
