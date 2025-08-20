namespace Lyt.Templator;

using System.Reflection.Metadata;

public sealed record class Parameter(
    string Tag,
    object Value,
    ParameterKind Kind = ParameterKind.Scalar)
{
    public string ToCode()
    {
        switch (this.Kind)
        {
            case ParameterKind.Scalar:
                if (this.Value is not string stringValue)
                {
                    throw new Exception("Unexpected Type for Parameter Value (string expected)");
                }

                return string.Format("var {0} = \"{1}\";", this.Tag, stringValue);

            case ParameterKind.Collection:
                if (this.Value is not IList<string> stringValues)
                {
                    throw new Exception("Unexpected Type for Parameter Value (IList<string> expected)");
                }

                StringBuilder sb = new (stringValues.Count * 50);
                sb.AppendLine(string.Format("List<System.String> {0} =", this.Tag));
                sb.AppendLine("[ ");
                foreach(string value in stringValues)
                {
                    sb.AppendLine(string.Format("\"{0}\",", value));
                }

                sb.AppendLine("] ;");
                return sb.ToString();

            default:
                throw new Exception("Unexpected Parameter Kind");
        }
    }

    public bool Validate(out string message)
    {
        message = string.Empty;
        if (string.IsNullOrWhiteSpace(this.Tag))
        {
            message = "Invalid Tag";
            return false;
        }

        if (this.Kind == ParameterKind.Scalar)
        {
            if (this.Value is not string stringValue)
            {
                throw new NotImplementedException("Scalar Value");
            }

            if (string.IsNullOrWhiteSpace(stringValue))
            {
                message = "Invalid Value";
                return false;
            }
        }
        else if (this.Kind == ParameterKind.Collection)
        {
            if (this.Value is not IList<string> collection)
            {
                throw new NotImplementedException("Scalar Value");
            }
        }
        else
        {
            throw new NotImplementedException("Kind");
        }

        return true;
    }
}

