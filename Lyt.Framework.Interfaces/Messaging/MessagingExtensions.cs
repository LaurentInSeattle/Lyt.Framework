namespace Lyt.Framework.Interfaces.Messaging;

using CommunityToolkit.Mvvm.Messaging;

public static class MessagingExtensions
{
    static MessagingExtensions() => Messenger = WeakReferenceMessenger.Default;

    public static IMessenger Messenger { get; set; }

    public static void Subscribe<TMessage>(this IRecipient<TMessage> subscriber) 
        where TMessage : class 
        => Messenger.Register<TMessage>(subscriber);

    public static void Publish<TMessage>(this TMessage message)
        where TMessage : class
        => Messenger.Send<TMessage>(message);

    public static void Unregister(this object recipient)
        => Messenger.UnregisterAll(recipient);

    public static void Unregister<TMessage>(this object recipient)
        where TMessage : class
        => Messenger.Unregister<TMessage>(recipient);
}
