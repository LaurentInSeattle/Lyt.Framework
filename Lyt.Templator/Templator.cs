namespace Lyt.Templator;

public sealed class Templator
{
    private readonly string template;

    public Templator(string template)
    {
        this.template = template;
    }

    public string Generate(Parameters parameters)
    {
        return template;
    }
}
