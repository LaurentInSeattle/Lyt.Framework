namespace Lyt.Translator.Service;

[JsonConverter(typeof(JsonStringEnumConverter<ProviderKey>))]
public enum ProviderKey
{
    Unknown = 0,

    Google,
    DeepL, 
    Microsoft,
}
