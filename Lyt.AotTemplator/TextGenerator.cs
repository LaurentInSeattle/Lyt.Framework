namespace Lyt.AotTemplator;

public sealed class TextGenerator(string template)
{
    // With ending space 
    private const string OpeningDelimiter = "<%= ";

    // Again With space 
    private const string ClosingDelimiter = " %>";

    private string template = template;

    public Tuple<bool, string> Generate(Parameters parameters)
    {
        if (!parameters.Validate(out string message))
        {
            return new Tuple<bool, string>(true, "Invalid parameters: " + message);
        }

        this.ProcessBuiltInVariables();
        var scalars =
            (from parameter in parameters
             where parameter.Kind == ParameterKind.Scalar
             select parameter).ToList();
        if (scalars.Count > 0)
        {
            this.ProcessScalars(scalars);
        }

        var collections =
            (from parameter in parameters
             where parameter.Kind == ParameterKind.Collection
             select parameter).ToList();
        if (collections.Count > 0)
        {
            this.ProcessCollections(collections);
        }

        return new Tuple<bool, string>(true, this.template);
    }

    private string Brace(string variableName)
        => string.Concat(OpeningDelimiter, variableName, ClosingDelimiter);
    
    private void ProcessBuiltInVariables()
    {
        // Just one for now 
        List<string> names =
            [
                "DateTime.Now",
            ];
        List<string> values = [];

        var now = DateTime.Now;
        values.Add(now.ToLongDateString() + "  " + now.ToLongTimeString());

        for (int i = 0; i < names.Count; ++i)
        {
            this.template = this.template.Replace(this.Brace(names[i]), values[i]);
        }
    }

    private void ProcessScalars(List<Parameter> parameters)
    {
        for (int i = 0; i < parameters.Count; ++i)
        {
            var parameter = parameters[i];
            string name = parameter.Tag;
            string value = parameter.StringValue;
            this.template = this.template.Replace(this.Brace(name), value);
        }
    }

    private void ProcessCollections(List<Parameter> parameters)
    {
        void ProcessCollection(Parameter parameter)
        {
            // TODO 
        }

        for (int i = 0; i < parameters.Count; ++i)
        {
            ProcessCollection(parameters[i]);
        }
    }
}
