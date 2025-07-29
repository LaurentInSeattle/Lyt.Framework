namespace Lyt.Templator;

public sealed class Parameters : List<Parameter>
{
    public Parameters() : base(20) { } 

    public bool Validate(out string message)
    {
        message = string.Empty;
        foreach (var parameter in this)
        {
            if (string.IsNullOrWhiteSpace(parameter.Tag))
            {
                message = "Invalid Tag"; 
                return false;
            }

            if (string.IsNullOrWhiteSpace(parameter.Value))
            {
                message = "Invalid Value";
                return false;
            }
        }

        return true;
    }
}
