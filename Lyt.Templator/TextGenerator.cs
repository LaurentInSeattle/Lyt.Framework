namespace Lyt.Templator;

public sealed class TextGenerator
{
    private const string ParameterStartTag = "<#";
    private const string ParameterEndTag = "#>";

    private readonly string template;

    public TextGenerator(string template) => this.template = template;

    public string Generate(Parameters parameters)
    {
        if (!parameters.Validate(out string message))
        {
            throw new Exception("Invalid parameters: " + message);
        }

        string result = new(template);
        foreach (var parameter in parameters)
        {
            string replace = 
                string.Concat(ParameterStartTag, " ", parameter.Tag, " ", ParameterEndTag);
            result = result.Replace(replace, parameter.Value);
        }

        return result;
    }
}
