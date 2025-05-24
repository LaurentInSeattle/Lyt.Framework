namespace Lyt.Framework.Interfaces.Localizing;

public sealed record class LanguageChangedMessage(string? OldLanguageKey, string NewLanguageKey);
