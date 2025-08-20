namespace Lyt.Templator;

public sealed class Parameters : List<Parameter>
{
    public Parameters() : base(20) { } 

    public bool Validate(out string message)
    {
        message = string.Empty;
        foreach (Parameter parameter in this)
        {
            bool success = parameter.Validate(out message); 
            if ( ! success )
            {
                return false;
            }
        }

        return true;
    }
}
