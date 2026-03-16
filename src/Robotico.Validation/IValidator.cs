namespace Robotico.Validation;

/// <summary>
/// Validates an instance of type <typeparamref name="T"/>. Validate returns a <see cref="Robotico.Result.Result"/> (success or validation error).
/// Use with Robotico.Mediator's <c>ValidationPipelineBehavior</c> to run validation before request handlers.
/// On failure, return <see cref="Robotico.Result.Result.ValidationError(IReadOnlyDictionary{string, string[]}, string?, string)"/> or use <see cref="ValidationErrors"/> to build the dictionary.
/// </summary>
/// <typeparam name="T">The type to validate.</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates <paramref name="instance"/>. Returns <see cref="Robotico.Result.Result.Success()"/> if valid; otherwise a failed Result (e.g. <see cref="Robotico.Result.Errors.ValidationError"/>).
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <returns>Success if valid; <see cref="Robotico.Result.Result.ValidationError(IReadOnlyDictionary{string, string[]}, string?, string)"/> if invalid.</returns>
    Robotico.Result.Result Validate(T instance);
}
