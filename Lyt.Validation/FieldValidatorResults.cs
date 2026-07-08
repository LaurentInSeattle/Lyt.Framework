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
    // ! If HasValue is true, then Value is not null, by design 
    internal T SafeValue 
        => this.HasValue ? this.Value! : throw new Exception("Should have checked 'HasValue'..."); 
}
