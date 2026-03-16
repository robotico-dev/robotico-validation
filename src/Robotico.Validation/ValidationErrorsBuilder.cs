using System.Collections.Immutable;

namespace Robotico.Validation;

/// <summary>
/// Builder to collect validation errors by field, then produce a Result or dictionary.
/// </summary>
public sealed class ValidationErrorsBuilder
{
    private readonly Dictionary<string, List<string>> _errors = new();

    /// <summary>
    /// Adds an error message for the given field.
    /// </summary>
    public ValidationErrorsBuilder Add(string fieldName, string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(fieldName);
        ArgumentNullException.ThrowIfNull(errorMessage);
        if (!_errors.TryGetValue(fieldName, out List<string>? list))
        {
            list = new List<string>();
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
