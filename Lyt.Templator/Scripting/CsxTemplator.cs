#define DEBUG_Verbose

namespace Lyt.Templator.Scripting;

public sealed class CsxTemplator
{
    public static async Task<Tuple<bool, string>> Generate(string template, Parameters parameters)
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
#if DEBUG_Verbose
                    Debug.WriteLine(parameter.Tag + " :  " + parameter.Value + " :  " + code);
#endif
                    sb.AppendLine(code);
                }

                script = CSharpScript.Create<string>(sb.ToString(), options);
#if DEBUG_Verbose
                Debug.WriteLine(script.Code);
#endif
                script = script.ContinueWith<string>(template, options);
            }

            if (script is null)
            {
                throw new Exception("Script should not be null: Template generation failed with no exception thrown");
            }

#if DEBUG_Verbose
            string scriptCode = script.Code;
            Debug.WriteLine("");
            Debug.WriteLine(scriptCode);
#endif
            var diagnostics = script.Compile();
            if (diagnostics.Length > 0)
            {
                string message = string.Join(Environment.NewLine, diagnostics.Select(d => $"// {d}"));
                Debug.WriteLine(message);
                if (Debugger.IsAttached) { Debugger.Break(); }
            }

            var state = await script.RunAsync();
            if (state is not null && state.ReturnValue is not null)
            {
                string? generated = state.ReturnValue.ToString();
                if (!string.IsNullOrWhiteSpace(generated))
                {
#if DEBUG_Verbose
                    Debug.WriteLine("");
                    Debug.WriteLine(generated);
#endif
                    return new Tuple<bool, string> (true, generated);
                }
            }

            throw new Exception("Template generation failed with no exception thrown");
        }
        catch (CompilationErrorException e)
        {
            Debug.WriteLine(e);
            string message = string.Join(Environment.NewLine, e.Diagnostics.Select(d => $"// {d}"));
            Debug.WriteLine(message);
            return new Tuple<bool, string>(false, message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return new Tuple<bool, string>(false, ex.Message);
        }
    }
}
