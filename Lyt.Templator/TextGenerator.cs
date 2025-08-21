namespace Lyt.Templator;

public sealed class TextGenerator(string template)
{
    private readonly string template = template;
    private readonly CsxTemplator templator = new();

    public Tuple<bool, string> Generate(Parameters parameters)
    {
        if (!parameters.Validate(out string message))
        {
            return new Tuple<bool, string>(true, "Invalid parameters: " + message);
        }

        return Task.Run(() => CsxTemplator.Generate(this.template, parameters)).Result;
    }
}
