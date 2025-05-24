namespace Lyt.Framework.Interfaces.Dispatching;

public interface IDispatch
{
    void OnUiThread(Action action);

    void OnUiThread<TArgs>(Action<TArgs> action, TArgs args); 
}
