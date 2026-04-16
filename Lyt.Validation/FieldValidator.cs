namespace Lyt.Validation;

public abstract class FieldValidator(
    Type targetType,
    string sourcePropertyName,
    bool allowEmpty = false,
    string messagePropertyName = "ValidationMessage",
    string emptyFieldMessage = "ValidationMessage",
    string failedToParseMessage = "ValidationMessage"
    )
{
    public const string DefaultEmptyFieldMessageKey = "Validation.EmptyFieldMessageKey";
    public const string DefaultFailedToParseMessageKey = "Validation.FailedToParseMessageKey";
    public const string DefaultEmptyFieldMessage = "This field is required.";
    public const string DefaultFailedToParseMessage = "This entry is invalid.";
    
    public Type TargetType { get; protected set; } = targetType;

    public string SourcePropertyName { get; protected set; } = sourcePropertyName;
    
    public bool AllowEmpty { get; protected set; } = allowEmpty;
    
    public string MessagePropertyName { get; protected set; } = messagePropertyName;
    
    public string EmptyFieldMessage { get; protected set; } = emptyFieldMessage;
    
    public string FailedToParseMessage { get; protected set; } = failedToParseMessage;

    internal abstract void Clear (IBindable viewModel);

    internal abstract FieldValidatorResults Validate(IBindable viewModel); 
}
