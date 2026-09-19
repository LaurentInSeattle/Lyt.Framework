namespace Lyt.Templator;

public sealed class TextGenerator(string template)
{
    private readonly string template = template;

    public Tuple<bool, string> Generate(Parameters parameters)
    {
        if (!parameters.Validate(out string message))
        {
            return new Tuple<bool, string>(true, "Invalid parameters: " + message);
        }

        return new Tuple<bool, string>(true, "Failed ");
    }
}
