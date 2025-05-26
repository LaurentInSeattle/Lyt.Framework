using Lyt.Utilities.Extensions;

namespace Lyt.Validation.Extensions;

public static class ValidationExtensions
{
    public static bool IsValid<TValidator, TType>(this TValidator maybeValidator, TType toValidate, out string message)
    {
        if (maybeValidator is null)
        {
            throw new Exception("Null validator");
        }

        if (!maybeValidator.GetType().DerivesFrom<AbstractValidator<TType>>())
        {
            throw new Exception("Invalid validator type: " + typeof(TType).FullName);
        }

        if (maybeValidator is not AbstractValidator<TType> validator)
        {
            throw new Exception("Null validator");
        }

        message = string.Empty;
        var result = validator.Validate(toValidate);
        if (!result.IsValid && result.Errors.Count > 0)
        {
            var firstError = result.Errors[0];
            message = firstError.ErrorMessage;
        }

        return result.IsValid;
    }

    public static string ShowValidationMessage(this IBindable viewModel, string? messagePropertyName, string message)
    {
        if (!string.IsNullOrWhiteSpace(messagePropertyName) &&
            !string.IsNullOrWhiteSpace(message))
        {
            // Localize message if a localizer is available 
            if (viewModel.CanLocalize)
            {
                // message = viewModel.Localizer.Lookup(message);
                message = viewModel.Localize(message);
            }

            // Set property: value comes first for Set
            viewModel.Set(message, messagePropertyName);
        }

        return message;
    }

    public static void ClearValidationMessage(this IBindable viewModel, string? messagePropertyName)
    {
        if (!string.IsNullOrWhiteSpace(messagePropertyName))
        {
            // Nothing to Localize, Set property: value comes first for Set
            viewModel.Set(string.Empty, messagePropertyName);
        }
    }

    public static bool TryParse<T>(this string s, out T? value, IFormatProvider? provider = null)
        where T : IParsable<T>
        => TryParse<T>(s, out value, provider);
}
