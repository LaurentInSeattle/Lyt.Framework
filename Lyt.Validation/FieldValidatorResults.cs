namespace Lyt.Validation;

internal record class FieldValidatorResults
(
    bool IsValid = false,
    bool HasValue = false,
    string Message = ""
);

internal sealed record class FieldValidatorResults<T> 
(
    T? Value = default,
    bool IsValid = false,
    bool HasValue = false,
    string Message = ""
) : FieldValidatorResults(IsValid , HasValue , Message )
{
    internal T SafeValue 
        => this.HasValue ? this.Value! : throw new Exception("Should have checked 'HasValue'..."); 
}
