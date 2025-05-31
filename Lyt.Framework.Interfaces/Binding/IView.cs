namespace Lyt.Framework.Interfaces.Binding; 

public interface IView
{
    object? DataContext { get; set; }

    bool IsVisible { get; set; }

    double Opacity { get; set; }

    bool IsEnabled { get; set; }
    
    bool IsHitTestVisible { get; set; }
}
