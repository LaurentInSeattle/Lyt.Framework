namespace Lyt.Framework.Interfaces.Messaging;

using CommunityToolkit.Mvvm.Messaging;

public static class MessagingExtensions
{
    public static void Subscribe<TMessage>(this IRecipient<TMessage> subscriber) 
        where TMessage : class 
        => WeakReferenceMessenger.Default.Register<TMessage>(subscriber);

    public static void Publish<TMessage>(this TMessage message)
        where TMessage : class
    {
        ArgumentNullException.ThrowIfNull(message);
        WeakReferenceMessenger.Default.Send<TMessage>(message);
    }

    public static void Unregister(object recipient)
    {
        ArgumentNullException.ThrowIfNull(recipient);
        WeakReferenceMessenger.Default.UnregisterAll(recipient);
    }

    public static void Unregister<TMessage>(this object recipient)
        where TMessage : class
    {
        ArgumentNullException.ThrowIfNull(recipient);
        WeakReferenceMessenger.Default.Unregister<TMessage>(recipient);
    }
}
