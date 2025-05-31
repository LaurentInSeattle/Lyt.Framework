
namespace Lyt.Framework.Interfaces.Messaging;

public sealed class NullMessenger : IMessenger
{
    public void Publish<TMessage>(TMessage message) 
        where TMessage : class { }

    public void Subscribe<TMessage>(Action<TMessage> action, bool withUiDispatch = false, bool doNotLog = false) 
        where TMessage : class { } 

    public void Unregister(object recipient) { }

    public void Unregister<TMessage>(object recipient) 
        where TMessage : class { }
}
