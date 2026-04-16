namespace Lyt.Validation;

public sealed class FieldValidator<T>(
    string sourcePropertyName,
    AbstractValidator<T>? validator = null,
    bool allowEmpty = false,
    string messagePropertyName = "ValidationMessage",
    string emptyFieldMessage = "ValidationMessage",
    string failedToParseMessage = "ValidationMessage" )
    : FieldValidator(typeof(T), sourcePropertyName, allowEmpty, messagePropertyName, emptyFieldMessage, failedToParseMessage)
{
    public AbstractValidator<T>? Validator { get; } = validator;

    internal override void Clear(IBindable viewModel)
    {
        // Clear
        // TODO: This ONLY clears string properties fields 
        viewModel.Set(this.SourcePropertyName, string.Empty);
        viewModel.ClearValidationMessage(this.MessagePropertyName);
    }

    internal override FieldValidatorResults<T> Validate(IBindable viewModel)
    {
        string ShowValidationMessage(string message)
            => viewModel.ShowValidationMessage(this.MessagePropertyName, message);

        // Get property value 
        // Case #1: Value is of type T or  Value can be converted to type T 
        // Case #2: Value is a string that can be converted to type T or T is string

        string propertyName = this.SourcePropertyName;
        bool valueIsFound = false;
        T? propertyValue = default;
        try
        {
            // Case #1: Value is of type T or  Value can be converted to type T 
            T? maybePropertyValue = viewModel.Get<T>(propertyName);
            if (maybePropertyValue is T valueOfT)
            {
                propertyValue = valueOfT;
                valueIsFound = true;
            }
        }
        catch
        {
            // Swallow
            viewModel.Logger.Info("Property is not of type " + typeof(T).Name);
        }

        if (!valueIsFound)
        {
            // Case #2: Value is a string that can be converted to type T or T is string
            string propertyText = string.Empty;
            string? maybePropertyText = viewModel.Get<string>(propertyName);
            bool isEmpty = string.IsNullOrWhiteSpace(maybePropertyText);
            if (!isEmpty)
            {
                // Trim 
                propertyText = maybePropertyText!;
                propertyText = propertyText.Trim();
                isEmpty = string.IsNullOrWhiteSpace(propertyText);
            }

            if (isEmpty)
            {
                // Clear white space noise, if any
                this.Clear(viewModel);
                if (this.AllowEmpty)
                {
                    return new FieldValidatorResults<T>(IsValid: true);
                }
                else
                {
                    string emptyMessage = this.EmptyFieldMessage;
                    emptyMessage = string.IsNullOrWhiteSpace(emptyMessage) ? DefaultEmptyFieldMessage : emptyMessage;
                    emptyMessage = ShowValidationMessage(emptyMessage);
                    return new FieldValidatorResults<T>(Message: emptyMessage);
                }
            }

            // Check if parsing is needed 
            if (typeof(string).Is<T>() && propertyText is T propertyString)
            {
                // Target type is string, no parsing needed
                propertyValue = propertyString;
                valueIsFound = true;
            }
            else
            {
                // Need to parse  
                bool isParsed = false;
                T? maybeValue = default;
                bool isIParsableOfT = 
                    typeof(T)
                    .GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IParsable<>));
                if (isIParsableOfT)
                {
                    // Type T implements IParsable<T>, we can parse directly

                    isParsed = propertyText.TryParseAny<T>(out maybeValue);
                } 
                else if (typeof(T).IsEnum)
                {
                    isParsed = Enum.TryParse(typeof(T), propertyText, ignoreCase: true, out object? enumValue);
                    if (isParsed && enumValue is T enumValueOfT)
                    {
                        maybeValue = enumValueOfT;
                    }
                }
                // else
                //  Type T does not implement IParsable<T> and is not an enum :
                //  consider trying to use TypeConverter as a fallback

                if (!isParsed || maybeValue is not T value)
                {
                    // failed to parse
                    string parseMessage = this.FailedToParseMessage;
                    parseMessage = string.IsNullOrWhiteSpace(parseMessage) ? DefaultFailedToParseMessage : parseMessage;
                    parseMessage = ShowValidationMessage(parseMessage);
                    var parseResults = new FieldValidatorResults<T>(Message: parseMessage);
                    return parseResults;
                }

                propertyValue = value;
                valueIsFound = true;
            }
        }

        if (!valueIsFound || propertyValue is null )
        {
            string emptyMessage = this.EmptyFieldMessage;
            emptyMessage = string.IsNullOrWhiteSpace(emptyMessage) ? DefaultEmptyFieldMessage : emptyMessage;
            emptyMessage = ShowValidationMessage(emptyMessage);
            return new FieldValidatorResults<T>(Message: emptyMessage);
        }

        // Now we have a value: Run the Fluent Validator if we have one, 
        // If no validator is specified, it's all done and valid...
        var validator = this.Validator;
        if (validator != null)
        {
            bool isValid = validator.IsValid(propertyValue, out string message);
            if (!isValid)
            {
                ShowValidationMessage(message);
                return
                    new FieldValidatorResults<T>(Message: message);
            }
        }

        viewModel.ClearValidationMessage(this.MessagePropertyName);
        return new FieldValidatorResults<T>(IsValid: true, HasValue: true, Value: propertyValue);
    }
}
