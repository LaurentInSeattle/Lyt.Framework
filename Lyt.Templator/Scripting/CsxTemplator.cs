namespace Lyt.Templator.Scripting;

using System.Text;

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
            var options = ScriptOptions.Default;
            if ( parameters.Count == 0)
            {
                script = CSharpScript.Create<string>(template, options);
            }
            else if (parameters.Count >= 1)
            {
                var parameter = parameters[0];

                // If provided 
                //      parameters.Clear();
                //      parameters.Add(new Parameter("PaletteKind", "Primary"));
                // Should generate: 
                //      var PaletteKind = "Primary";

                StringBuilder sb = new(parameters.Count * 50);
                for (int i = 0; i < parameters.Count; ++i)
                {
                    parameter = parameters[i];
                    string code =
                        string.Format(
                            "var {0} = \"{1}\";",
                            parameter.Tag, parameter.Value);
                    Debug.WriteLine(parameter.Tag + " :  " + parameter.Value + " :  " + code);
                    sb.AppendLine(code); 
                }

                script = CSharpScript.Create<string>(sb.ToString());
                script = script.ContinueWith<string>(template, options);

                //script.ContinueWith<string>(code);
                //string code =
                //    string.Format(
                //        "var {0} = \"{1}\";",
                //        parameter.Tag, parameter.Value); 
                //Debug.WriteLine(parameter.Tag + " :  " + parameter.Value + " :  " + code);
                //script = CSharpScript.Create<string>(code);
                //if (parameters.Count >= 1)
                //{
                //    // Note: Start at ONE! 
                //    for (int i = 1; i < parameters.Count; ++i )
                //    {
                //        parameter = parameters[i];
                //        code =
                //            string.Format(
                //                "var {0} = \"{1}\";",
                //                parameter.Tag, parameter.Value);
                //        Debug.WriteLine(parameter.Tag + " :  " + parameter.Value + " :  " + code);
                //        script.ContinueWith<string>(code); 
                //    }
                //}

            }

            if (script is null)
            {
                throw new Exception("Script should not be null: Template generation failed with no exception thrown");
            }

            string scriptCode = script.Code;
            Debug.WriteLine("");
            Debug.WriteLine(scriptCode);

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
            return string.Join(Environment.NewLine, e.Diagnostics.Select(d => $"// {d}"));
        }
        catch  (Exception ex) 
        { 
            Debug.WriteLine(ex);
            return string.Empty;
        }
    }
}
