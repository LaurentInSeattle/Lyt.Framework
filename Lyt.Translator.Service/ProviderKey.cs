namespace Lyt.Translator.Service;

[JsonConverter(typeof(JsonStringEnumConverter<ProviderKey>))]
public enum ProviderKey
{
    Unknown = 0,

    LibreTranslate,
    Google,
    DeepL, 
    Microsoft,
}
