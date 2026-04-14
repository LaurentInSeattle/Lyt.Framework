namespace Lyt.Validation;

public sealed class FormValidator<T>(FormValidatorParameters<T> parameters) :
    IFormValidator<T>
    where T : class, new()
{
    private readonly FormValidatorParameters<T> parameters = parameters;

    private readonly List<FieldValidator> fieldValidators = [.. parameters.FieldValidators];

    private T? value;

    public Type TargetType => typeof(T);

    public bool HasValue { get; private set; }

    public T Value
        => this.HasValue && this.value is not null ?
            this.value :
            throw new InvalidOperationException("No value, should have checked 'HasValue'.");

    public void Clear(IBindable viewModel)
    {
        foreach (var fieldValidator in this.fieldValidators)
        {
            fieldValidator.Clear(viewModel);
        }

        viewModel.ClearValidationMessage(this.parameters.MessagePropertyName);
        this.SetFormValidProperty(viewModel, isValid: false);
        _ = this.TryFocus(viewModel);
    }

    public bool TryFocus(IBindable viewModel)
    {
        string? focusFieldName = this.parameters.FocusFieldName;
        if (!string.IsNullOrWhiteSpace(focusFieldName))
        {
            bool focused = viewModel.TryFocusField(focusFieldName);
            if (!focused)
            {
                viewModel.Logger.Warning(viewModel.GetType().Name + ": Focus has not been set.");
            }

            return focused;
        }

        return false;
    }

    public FormValidatorResults<T> Validate(IBindable viewModel)
    {
        string ShowValidationMessage(string message)
            => viewModel.ShowValidationMessage(this.parameters.MessagePropertyName, message);

        void SetFormValidProperty(bool isValid)
            => this.SetFormValidProperty(viewModel, isValid);

        // Step #1 : Validate field by field 
        List<FieldValidatorResults> results = [];
        foreach (var fieldValidator in this.fieldValidators)
        {
            var result = fieldValidator.Validate(viewModel);
            if (!result.IsValid)
            {
                SetFormValidProperty(isValid: false);
                return new FormValidatorResults<T>(IsValid: false);
            }

            results.Add(result);
        }

        // Step #2: all fields valid, run the validator if one is provided 
        // 2-a Create object from validated fields, property names should match 
        T formValue = new();
        for (int i = 0; i < results.Count; ++i)
        {
            var fieldValidator = this.fieldValidators[i];
            var result = results[i];

            // Copy validated property value into new object 
            string propertyName = fieldValidator.Parameters.SourcePropertyName;
#pragma warning disable CA1507 // Use nameof to express symbol names
            // VS does not understand it... 
            object? propertyValue = result.InvokeGetProperty("Value");
#pragma warning restore CA1507 

            // Perform data conversions from string to numbers as needed
            if (formValue.TryGetPropertyType(propertyName, out Type? propertyType))
            {
                if (propertyType != typeof(string))
                {
                    if (TryParse(propertyValue, propertyType, out object? parsedValue))
                    {
                        propertyValue = parsedValue;
                    }
                    else
                    {
                        return new FormValidatorResults<T>(IsValid: false, Message: $"Failed to parse property '{propertyName}'");
                    }
                }
            }
            else
            {
                return new FormValidatorResults<T>(IsValid: false, Message: $"Property '{propertyName}': Type not found.");
            }

            formValue.InvokeSetProperty(propertyName, propertyValue);
        }

        // 2-b Validate the resulting object if a validator is provided 
        var maybeValidator = this.parameters.FormValidator;
        if (maybeValidator is not null)
        {
            bool isValid = maybeValidator.IsValid(formValue, out string message);
            if (!isValid)
            {
                SetFormValidProperty(isValid: false);
                ShowValidationMessage(message);
                return new FormValidatorResults<T>(IsValid: false, Message: message);
            }
        }

        // All passed, fully validated 
        SetFormValidProperty(isValid: true);
        this.HasValue = true;
        this.value = formValue;
        return new FormValidatorResults<T>(IsValid: true, HasValue: true, Value: formValue);
    }

    private void SetFormValidProperty(IBindable viewModel, bool isValid)
    {
        string propertyName = this.parameters.FormValidPropertyName;
        if (!string.IsNullOrWhiteSpace(propertyName))
        {
            viewModel.InvokeSetProperty(propertyName, isValid);
        }
    }

    private static bool TryParse(object? propertyValue, Type targetType, [NotNullWhen(true)] out object? result)
    {
        result = null;
        if ((propertyValue is null) || (propertyValue.GetType() != typeof(string)))
        {
            return false;
        }

        string? value = (string?)propertyValue;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            if (targetType == typeof(int))
            {
                if (int.TryParse(value, out int intValue))
                {
                    result = intValue;
                    return true;
                }
            }
            else if (targetType == typeof(uint))
            {
                if (uint.TryParse(value, out uint uintValue))
                {
                    result = uintValue;
                    return true;
                }
            }
            else if (targetType == typeof(long))
            {
                if (long.TryParse(value, out long longValue))
                {
                    result = longValue;
                    return true;
                }
            }
            else if (targetType == typeof(ulong))
            {
                if (ulong.TryParse(value, out ulong ulongValue))
                {
                    result = ulongValue;
                    return true;
                }
            }
            else if (targetType == typeof(double))
            {
                if (double.TryParse(value, out double doubleValue))
                {
                    result = doubleValue;
                    return true;
                }
            }
            else if (targetType == typeof(float))
            {
                if (float.TryParse(value, out float floatValue))
                {
                    result = floatValue;
                    return true;
                }
            }
            else if (targetType == typeof(decimal))
            {
                if (decimal.TryParse(value, out decimal decimalValue))
                {
                    result = decimalValue;
                    return true;
                }
            }
            else
            {
                // Add more types as needed, for now failing for unsupported types
                return false;
            }
        }
        catch (Exception ex)
        {
            // Ignore parsing exceptions and return false
            Debug.WriteLine("Parsing Exception: " + ex);
        }

        return false;
    }
}