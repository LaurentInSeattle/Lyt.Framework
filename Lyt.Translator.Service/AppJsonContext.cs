namespace Lyt.Translator.Service;

[JsonSourceGenerationOptions(WriteIndented = false)]

// Google classes  
[JsonSerializable(typeof(TranslationResponse))]
[JsonSerializable(typeof(Sentence))]

// Libre Translate classes  
[JsonSerializable(typeof(TranslateParameter))]
[JsonSerializable(typeof(TranslateResult))]
[JsonSerializable(typeof(TranslateFileResult))]
[JsonSerializable(typeof(ErrorResult))]
public partial class AppJsonContext : JsonSerializerContext
{
}
