namespace Lyt.Templator;

public sealed record class Parameter (
    string Tag,
    string Value = "", 
    ParameterKind Kind = ParameterKind.Basic);
