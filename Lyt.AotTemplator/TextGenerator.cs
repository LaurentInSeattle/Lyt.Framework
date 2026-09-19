namespace Lyt.AotTemplator;

using System.Drawing;

public sealed class TextGenerator(string template)
{
	// With ending space 
	private const string OpeningDelimiter = "<%= ";

	// Again With space 
	private const string ClosingDelimiter = " %>";

	// For collections variables 
	// Example: 
	//
	//  <%=BEGIN_LOOP:Colors%>
	//	    <Color x:Key="Color_<%= LoopIndex %>"><%= LoopValue %></Color>
	//	    <SolidColorBrush x:Key="Brush_<%= LoopIndex %>" Color ="{StaticResource Color_<%= LoopIndex %>}" />"
	//  <%=END_LOOP%>
	//
	private const string OpeningLoopStartDelimiter = "<%=BEGIN_LOOP:";
	private const string OpeningLoopEndDelimiter = "%>";
	private const string ClosingLoopDelimiter = "<%=END_LOOP%>";
	private const string LoopIndexVariable = "<%= LoopIndex %>";
	private const string LoopValueVariable = "<%= LoopValue %>";

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

		char[] lineSeparators = ['\n', '\r']; 
		string[] templateLines = this.template.Split(lineSeparators, StringSplitOptions.RemoveEmptyEntries); 
		
		void ProcessCollection(Parameter parameter)
		{
			if (parameter.Value is not IList<string> stringValues )
			{
				return; 
			}

			// Find any begin loop delimiter in templates lines matching the parameter tag
			bool found = false;
			int startRepeatLine = -1;
			for (int lineIndex = 0; lineIndex < templateLines.Length; ++lineIndex)
			{
				string line = templateLines[lineIndex];
				if (string.IsNullOrWhiteSpace(line))
				{
					continue;
				}

				int start = line.IndexOf(OpeningLoopStartDelimiter);
				int end = line.IndexOf(OpeningLoopEndDelimiter);
				if (start == -1 || end == -1)
				{
					continue;
				}

				int length = end - start - OpeningLoopStartDelimiter.Length;
				string collectionName = line.Substring(start + OpeningLoopStartDelimiter.Length, length);
				if (collectionName == parameter.Tag)
				{
					found = true;
					startRepeatLine = lineIndex + 1;
					break;
				}
			}

			if (!found)
			{
				return;
			}

			// Now find the matching end loop tag 
			found = false; 
			int endRepeatLine = -1; 
			for (int lineIndex = startRepeatLine; lineIndex < templateLines.Length; ++lineIndex)
			{
				string line = templateLines[lineIndex];
				if (string.IsNullOrWhiteSpace(line))
				{
					continue;
				}

				if ( line.Contains(ClosingLoopDelimiter))
				{
					found = true;
					endRepeatLine = lineIndex - 1;
					break; 
				}
			}

			if (!found)
			{
				return;
			}

			List<string> repeatLines = []; 
			for ( int repeatIndex = startRepeatLine; repeatIndex <= endRepeatLine; ++ repeatIndex)
			{
				repeatLines.Add(templateLines[repeatIndex]); 
			}

			List<string> linesToAdd = [];
			for ( int loopIndex = 0; loopIndex < stringValues.Count; ++ loopIndex)
			{
				string loopIndexString = loopIndex.ToString("D");
				string loopValueString = stringValues[loopIndex]; 
				foreach ( string line in repeatLines)
				{
					string lineToAdd = line.Replace(LoopIndexVariable, loopIndexString);
					lineToAdd = lineToAdd.Replace(LoopValueVariable, loopValueString);
					linesToAdd.Add(lineToAdd); 
				}
			}

			// Rebuild template lines 
			List<string> newLines = []; 
			for (int i = 0; i < startRepeatLine - 1 ; i++)
			{
				newLines.Add(templateLines[i]); 
			}

			foreach(string newLine in linesToAdd)
			{
				newLines.Add(newLine);
			}

			for (int i = endRepeatLine + 2; i < templateLines.Length; i++)
			{
				newLines.Add(templateLines[i]);
			}

			templateLines = newLines.ToArray(); 
		}

		for (int i = 0; i < parameters.Count; ++i)
		{
			ProcessCollection(parameters[i]);
		}

		// Recreate a single string from the lines 
		StringBuilder stringBuilder = new(50 * templateLines.Length); 
		foreach(string line in templateLines)
		{
			stringBuilder.AppendLine(line);
		}

		this.template = stringBuilder.ToString(); 
	}
}
