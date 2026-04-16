namespace Lyt.Validation;

public record class FieldValidatorParameters
(
    string SourcePropertyName,
    bool AllowEmpty = false,
    string MessagePropertyName = "ValidationMessage",
    string EmptyFieldMessage = "ValidationMessage",
    string FailedToParseMessage = "ValidationMessage"
);

public sealed record class FieldValidatorParameters<T>
(
    string SourcePropertyName,
    AbstractValidator<T>? Validator = null,
    bool AllowEmpty = false,
    string MessagePropertyName = "ValidationMessage",
    string EmptyFieldMessage = "ValidationMessage",
    string FailedToParseMessage = "ValidationMessage"
) : FieldValidatorParameters(SourcePropertyName, AllowEmpty, MessagePropertyName, EmptyFieldMessage, FailedToParseMessage);

