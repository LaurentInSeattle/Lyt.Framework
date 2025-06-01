namespace Lyt.Framework.Interfaces.Binding;

using Lyt.Framework.Interfaces.Logging;

public interface IBindable
{
    ILogger Logger { get; }

    bool TryFocusField(string fieldName);

    bool CanLocalize { get; }

    string Localize(string message);

    T? Get<T>(string propertyName); 

    void Set<T>(string propertyName, T value); 
}
