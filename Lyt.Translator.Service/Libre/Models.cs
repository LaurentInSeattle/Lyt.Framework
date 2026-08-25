namespace Lyt.Translator.Service.Libre;

public class LibreTranslateOptions
{
    public Uri? Url { get; init; }

    public string? ApiKey { get; init; }

    public TranslationFormat? Format { get; init; }

    public int Alternatives { get; init; }
}

public record class DetectParameter(
    [property: JsonPropertyName("q")]
    string Text,
    [property: JsonPropertyName("api_key")]
    string? ApiKey);

public record class DetectResult(double Confidence, string Language);

public record class LanguageResult(string Code, string Name, IList<string> Targets);

public record class TranslateParameter(
    [property: JsonPropertyName("q")]
    string Text,
    [property: JsonPropertyName("source")]
    string Source,
    [property: JsonPropertyName("target")]
    string Target,
    [property: JsonPropertyName("format")]
    TranslationFormat? Format,
    [property: JsonPropertyName("alternatives")]
    int Alternatives,
    [property: JsonPropertyName("api_key")]
    string? ApiKey);

[JsonConverter(typeof(JsonStringEnumConverter<TranslationFormat>))]
public enum TranslationFormat
{
    text,
    html,
}

public record class TranslateResult(
    [property: JsonPropertyName("translatedText")]
    string TranslatedText,
    [property: JsonPropertyName("alternatives")]
    IList<string>? Alternatives);

public record class TranslateFileResult(string TranslatedFileUrl);

public class LibreTranslateResult<TResult>
    where TResult : class
{
    private readonly TResult? result;
    private readonly string? error;

    private LibreTranslateResult(bool isSuccessful, TResult? result, string? error)
    {
        IsSuccessful = isSuccessful;
        this.result = result;
        this.error = error;
    }

    public static LibreTranslateResult<TResult> Success(TResult result)
    {
        return new LibreTranslateResult<TResult>(true, result, null);
    }

    public static LibreTranslateResult<TResult> Failed(string error)
    {
        return new LibreTranslateResult<TResult>(false, null, error);
    }

    public bool IsSuccessful { get; }

    public TResult Result
    {
        get => result ?? throw new InvalidOperationException();
    }

    public string Error
    {
        get => error ?? throw new InvalidOperationException();
    }
}

public record class ErrorResult(string Error);