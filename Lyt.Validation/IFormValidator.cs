namespace Lyt.Validation;

public interface IFormValidator
{
    bool HasValue { get; }

    void Clear(IBindable viewModel);

    bool TryFocus(IBindable viewModel); 
}

public interface IFormValidator<T> : IFormValidator where T : class, new()
{
    T Value { get; }

    FormValidatorResults<T> Validate(IBindable viewModel); 
}
