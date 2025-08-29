namespace Lyt.Framework.Interfaces.Messaging;

using CommunityToolkit.Mvvm.Messaging;

public sealed class NullMessenger : IMessenger
{
    public void Reset() { }

    public void Cleanup() { }
    
    public void Register<TRecipient, TMessage, TToken>(
        TRecipient recipient, TToken token, MessageHandler<TRecipient, TMessage> handler)
        where TRecipient : class
        where TMessage : class
        where TToken : IEquatable<TToken>
    { 
    }

    public bool IsRegistered<TMessage, TToken>(object recipient, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
        => false;

    public TMessage Send<TMessage, TToken>(TMessage message, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
        => message;

    public void Unregister<TMessage, TToken>(object recipient, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken> { }

    public void UnregisterAll(object recipient) { }

    public void UnregisterAll<TToken>(object recipient, TToken token) where TToken : IEquatable<TToken> { }
}
