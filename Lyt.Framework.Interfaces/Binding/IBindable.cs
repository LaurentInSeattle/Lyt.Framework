namespace Lyt.Framework.Interfaces.Binding;

using Lyt.Framework.Interfaces.Logging;

public interface IBindable
{
    ILogger Logger { get; }

    bool CanFocus { get; }

    bool TryFocusField(string fieldName);

    bool CanLocalize { get; }

    string Localize(string message, bool failSilently = false);

    //  Most like useless now that we have move on the MSFT CTK 
    //  Used by the Validation Framework 

    T? Get<T>(string propertyName);

    void Set<T>(string propertyName, T value);
}
