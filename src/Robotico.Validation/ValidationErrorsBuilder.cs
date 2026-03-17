using System.Collections.Immutable;

namespace Robotico.Validation;

/// <summary>
/// Builder to collect validation errors by field, then produce a Result or dictionary.
/// </summary>
/// <remarks>
/// This type is not thread-safe. Use from a single thread or synchronize externally.
/// </remarks>
public sealed class ValidationErrorsBuilder
{
    private readonly Dictionary<string, List<string>> _errors = [];

    /// <summary>
    /// Adds an error message for the given field.
    /// </summary>
    /// <param name="fieldName">The field name to associate with the error.</param>
    /// <param name="errorMessage">The error message to add.</param>
    /// <returns>This builder for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fieldName"/> or <paramref name="errorMessage"/> is null.</exception>
    public ValidationErrorsBuilder Add(string fieldName, string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(fieldName);
        ArgumentNullException.ThrowIfNull(errorMessage);
        if (!_errors.TryGetValue(fieldName, out List<string>? list))
        {
            list = [];
            _errors[fieldName] = list;
        }

        list.Add(errorMessage);
        return this;
    }

    /// <summary>
    /// Returns true if any errors have been added.
    /// </summary>
    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    /// Builds an immutable dictionary of field name to error messages array.
    /// </summary>
    /// <returns>An immutable dictionary of field names to error message arrays.</returns>
    public IReadOnlyDictionary<string, string[]> ToDictionary()
    {
        ImmutableDictionary<string, string[]>.Builder builder = ImmutableDictionary.CreateBuilder<string, string[]>();
        foreach (KeyValuePair<string, List<string>> kv in _errors)
        {
            builder.Add(kv.Key, kv.Value.ToArray());
        }

        return builder.ToImmutable();
    }

    /// <summary>
    /// If no errors were added, returns success; otherwise returns a validation error Result.
    /// </summary>
    /// <param name="message">Optional summary message for the validation error.</param>
    /// <param name="code">Error code. Defaults to "VAL_FAILED".</param>
    /// <returns><see cref="Robotico.Result.Result.Success"/> if no errors; otherwise a failed Result with <see cref="Robotico.Result.Errors.ValidationError"/>.</returns>
    public Robotico.Result.Result ToResult(string? message = null, string code = "VAL_FAILED")
    {
        if (_errors.Count == 0)
        {
            return Robotico.Result.Result.Success();
        }

        IReadOnlyDictionary<string, string[]> dict = ToDictionary();
        return Robotico.Result.Result.ValidationError(dict, message, code);
    }
}
