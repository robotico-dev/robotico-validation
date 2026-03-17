namespace Robotico.Validation;

/// <summary>
/// Helper to build a field-to-messages dictionary for <see cref="Robotico.Result.Result.ValidationError(IReadOnlyDictionary{string, string[]}, string?, string)"/>.
/// </summary>
public static class ValidationErrors
{
    /// <summary>
    /// Creates a validation error result from a dictionary of field names to error messages.
    /// </summary>
    /// <param name="errors">Field name to array of error messages.</param>
    /// <param name="message">Optional summary message. If null, a default is generated.</param>
    /// <param name="code">Error code. Defaults to "VAL_FAILED".</param>
    /// <returns>A failed <see cref="Robotico.Result.Result"/> with a <see cref="Robotico.Result.Errors.ValidationError"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors"/> is empty (at least one field error is required).</exception>
    public static Robotico.Result.Result ToResult(IReadOnlyDictionary<string, string[]> errors, string? message = null, string code = "VAL_FAILED")
    {
        ArgumentNullException.ThrowIfNull(errors);
        if (errors.Count == 0)
        {
            throw new ArgumentException("At least one validation error is required.", nameof(errors));
        }

        return Robotico.Result.Result.ValidationError(errors, message, code);
    }

    /// <summary>
    /// Creates a single-field validation error result (one or more messages).
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    /// <param name="errorMessages">One or more error messages for the field.</param>
    public static Robotico.Result.Result ForField(string fieldName, params string[] errorMessages)
    {
        ArgumentNullException.ThrowIfNull(fieldName);
        ArgumentNullException.ThrowIfNull(errorMessages);
        if (errorMessages.Length == 0)
        {
            throw new ArgumentException("At least one error message is required", nameof(errorMessages));
        }

        Dictionary<string, string[]> errors = new()
        {
            [fieldName] = errorMessages
        };
        string summary = errorMessages.Length == 1 ? errorMessages[0] : $"{errorMessages.Length} validation errors";
        return Robotico.Result.Result.ValidationError(errors, $"Validation failed for {fieldName}: {summary}", "VAL_" + fieldName.ToUpperInvariant());
    }

    /// <summary>
    /// Creates a new builder to collect field errors.
    /// </summary>
    public static ValidationErrorsBuilder CreateBuilder() => new();
}
