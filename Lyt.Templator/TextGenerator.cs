namespace Lyt.Templator;

public sealed class TextGenerator(string template)
{
    private readonly string template = template;
    private readonly CsxTemplator templator = new();

    public string Generate(Parameters parameters)
    {        
        string result = Task.Run(() => this.templator.Generate(this.template, parameters)).Result;

        //if (!parameters.Validate(out string message))
        //{
        //    throw new Exception("Invalid parameters: " + message);
        //}

        //string result = new(template);
        //foreach (var parameter in parameters)
        //{
        //    string replace = 
        //        string.Concat(ParameterStartTag, " ", parameter.Tag, " ", ParameterEndTag);
        //    result = result.Replace(replace, parameter.Value);
        //}

        return result;
    }
}
