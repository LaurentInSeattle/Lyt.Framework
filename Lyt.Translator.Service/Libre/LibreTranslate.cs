namespace Lyt.Translator.Service.Libre;

using static System.Net.WebRequestMethods;

public class LibreTranslate
{
    private readonly HttpClient client;
    private readonly LibreTranslateOptions options; 

    public LibreTranslate(LibreTranslateOptions options)
    {
        this.options = options;
        this.client = new()
        {
            BaseAddress = options.Url ?? new Uri("http://localhost:5000"),
            Timeout = 
                Debugger.IsAttached ?
                    TimeSpan.FromSeconds(120) : 
                    TimeSpan.FromSeconds(20)
        };
    }

    public void Dispose() => ((IDisposable)client).Dispose();

    public async Task<LibreTranslateResult<TranslateFileResult>> TranslateFile(
        string fileName, byte[] file, string source, string target)
    {
        using var form = new MultipartFormDataContent();

        var fileContent = new ByteArrayContent(file);
        form.Add(fileContent, nameof(file), fileName);
        form.Add(new StringContent(source), nameof(source));
        form.Add(new StringContent(target), nameof(target));
        if (options.ApiKey is not null)
        {
            form.Add(new StringContent(options.ApiKey), "api_key");
        }

        return await PostUpload("/translate_file", form);
    }

    public async Task<LibreTranslateResult<TranslateResult>> Translate(string text, string source, string target)
    {
        try
        {
            var parameter = new TranslateParameter(text, source, target, options.Format, options.Alternatives, options.ApiKey);
            string json = JsonSerializer.Serialize(parameter, AppJsonContext.Default.TranslateParameter);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");             
            string url = @"http://localhost:5000" + "/translate" ; 
            using var response = await this.client.PostAsync(url, stringContent);
            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            string jsonText = reader.ReadToEnd();
            var responseObject = JsonSerializer.Deserialize(jsonText, AppJsonContext.Default.TranslateResult);
            if (responseObject is TranslateResult translateResult)
            {
                return LibreTranslateResult<TranslateResult>.Success(translateResult);
            }

            return LibreTranslateResult<TranslateResult>.Failed("Unknown error");
        }
        catch (Exception ex)
        {
            return LibreTranslateResult<TranslateResult>.Failed(ex.Message);
        }
    }

    public async Task<LibreTranslateResult<TranslateFileResult>> PostUpload(string requestUri, MultipartFormDataContent content)
    {
        try
        {
            // LATER 

            return LibreTranslateResult<TranslateFileResult>.Failed("Unknown error");
        }
        catch (Exception ex)
        {
            return LibreTranslateResult<TranslateFileResult>.Failed(ex.Message);
        }
    }
}

