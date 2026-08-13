namespace Lyt.Framework.Interfaces.Dispatching;

public interface IDispatch
{
    void OnIdle(Action action);

    void OnUiThread(Action action);

    void OnUiThread<TArgs>(Action<TArgs> action, TArgs args); 
}
