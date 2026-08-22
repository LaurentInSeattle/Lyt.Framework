namespace Lyt.Translator.Service.Google;

/*

Sample : 

    {
        "sentences":
        [
            {
                "trans":"Bonjour le monde",
                "orig":"Hello World",
                "backend":10
            }
        ],
        "src":"en",
        "spell":
        { 
        }
    }

*/

public class Sentence
{
    [JsonPropertyName("trans")]
    public string? Translation { get; init; }
}

public class TranslationResponse
{
    [JsonPropertyName("sentences")]
    public List<Sentence>? Sentences { get; init; }
}
