namespace Lyt.Mvvm; 

public interface IView
{
    object? DataContext { get; set; }

    bool IsVisible { get; set; }
}
