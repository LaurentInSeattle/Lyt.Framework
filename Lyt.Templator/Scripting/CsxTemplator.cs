namespace Lyt.Templator.Scripting;

public sealed class CsxTemplator
{
    public async Task<string> Generate(string template, Parameters parameters)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new Exception("Empty template code");
            }

            Script<string>? script = null;
            var options = ScriptOptions
                .Default
                .AddReferences(typeof(List<string>).Assembly)
                .AddImports("System.Collections.Generic");
            if (parameters.Count == 0)
            {
                script = CSharpScript.Create<string>(template, options);
            }
            else if (parameters.Count >= 1)
            {
                // If provided 
                //      parameters.Clear();
                //      parameters.Add(new Parameter("PaletteKind", "Primary"));
                // Should generate: 
                //      var PaletteKind = "Primary";

                StringBuilder sb = new(parameters.Count * 50);
                foreach (Parameter parameter in parameters)
                {
                    string code = parameter.ToCode();
                    Debug.WriteLine(parameter.Tag + " :  " + parameter.Value + " :  " + code);
                    sb.AppendLine(code);
                }

                script = CSharpScript.Create<string>(sb.ToString(), options);
                script = script.ContinueWith<string>(template, options);
            }

            if (script is null)
            {
                throw new Exception("Script should not be null: Template generation failed with no exception thrown");
            }

            string scriptCode = script.Code;
            Debug.WriteLine("");
            Debug.WriteLine(scriptCode);

            var diagnostics = script.Compile();
            var state = await script.RunAsync();
            if (state is not null && state.ReturnValue is not null)
            {
                string? generated = state.ReturnValue.ToString();
                if (!string.IsNullOrWhiteSpace(generated))
                {
                    Debug.WriteLine("");
                    Debug.WriteLine(generated);
                    return generated;
                }
            }

            throw new Exception("Template generation failed with no exception thrown");
        }
        catch (CompilationErrorException e)
        {
            Debug.WriteLine(e);
            string message = string.Join(Environment.NewLine, e.Diagnostics.Select(d => $"// {d}"));
            Debug.WriteLine(message);
            return message; 
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return string.Empty;
        }
    }
}
