namespace Lyt.Framework.Interfaces.Binding;

using Lyt.Framework.Interfaces.Logging;

public interface IBindable
{
    ILogger Logger { get; }

    bool TryFocus(string propertyName);

    bool CanLocalize { get; }

    string Localize(string message);

    void Set(string message, string messagePropertyName);

    string? Get(string sourcePropertyName);
}
