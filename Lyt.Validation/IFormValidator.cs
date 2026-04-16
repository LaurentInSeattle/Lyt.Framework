namespace Lyt.Validation;

public interface IFormValidator<T> where T : class, new()
{
    T Value { get; }

    bool HasValue { get; }

    void Clear(IBindable viewModel);

    bool TryFocus(IBindable viewModel);

    FormValidatorResults<T> Validate(IBindable viewModel); 
}
