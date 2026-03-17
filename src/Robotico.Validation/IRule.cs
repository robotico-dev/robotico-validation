namespace Robotico.Validation;

/// <summary>
/// A reusable validation rule for type <typeparamref name="T"/>. Returns <see cref="Robotico.Result.Result"/> (success or validation error).
/// Compose rules in an <see cref="IValidator{T}"/> or use <see cref="Rule"/> to combine multiple rules.
/// </summary>
/// <typeparam name="T">The type to validate.</typeparam>
/// <remarks>
/// <para><b>When to use</b>: Use <see cref="IRule{T}"/> for small, reusable checks (e.g. "email format", "max length") that can be shared across validators. Use <see cref="IValidator{T}"/> for the full validation of an instance (often composing several rules).</para>
/// <para><b>Result</b>: On failure, return <see cref="Robotico.Result.Result.ValidationError(IReadOnlyDictionary{string, string[]}, string?, string)"/> or <see cref="ValidationErrors.ForField"/> / <see cref="ValidationErrorsBuilder"/>.</para>
/// </remarks>
public interface IRule<in T>
{
    /// <summary>
    /// Validates <paramref name="instance"/> for this rule. Returns success if the rule passes; otherwise a failed Result (e.g. <see cref="Robotico.Result.Errors.ValidationError"/>).
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <returns>Success if the rule passes; otherwise a failed Result with validation error(s).</returns>
    Robotico.Result.Result Validate(T instance);
}
