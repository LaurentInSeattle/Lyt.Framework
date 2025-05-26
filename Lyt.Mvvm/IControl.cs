namespace Lyt.Mvvm; 

public interface IControl
{
    event EventHandler<EventArgs> Loaded;

    object? DataContext { get; set; }
}
