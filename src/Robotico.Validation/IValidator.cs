namespace Robotico.Validation;

/// <summary>
/// Validates an instance of type <typeparamref name="T"/>. Validate returns a Result (success or validation error).
/// </summary>
/// <typeparam name="T">The type to validate.</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates <paramref name="instance"/>. Returns Result.Success() if valid; otherwise a failed Result (e.g. validation error).
    /// </summary>
    Robotico.Result.Result Validate(T instance);
}
