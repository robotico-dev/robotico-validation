namespace Robotico.Validation;

/// <summary>
/// Factory and composition helpers for <see cref="IRule{T}"/>.
/// </summary>
public static class Rule
{
    /// <summary>
    /// Runs all <paramref name="rules"/> on <paramref name="instance"/> and combines results: success only if every rule passes; otherwise a single validation error with all field errors collected.
    /// </summary>
    /// <param name="instance">The instance to validate. Passed to each rule; may be null if rules accept it.</param>
    /// <param name="rules">The rules to run (in order).</param>
    /// <returns>Success if all rules pass; otherwise a failed Result with combined validation errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="rules"/> is null.</exception>
    public static Robotico.Result.Result ValidateAll<T>(T instance, IEnumerable<IRule<T>> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        ValidationErrorsBuilder? builder = null;
        foreach (IRule<T> rule in rules)
        {
            Robotico.Result.Result r = rule.Validate(instance);
            if (r.IsError(out Robotico.Result.Errors.IError? err) && err is Robotico.Result.Errors.ValidationError ve)
            {
                builder ??= ValidationErrors.CreateBuilder();
                foreach (KeyValuePair<string, string[]> kv in ve.Errors)
                {
                    foreach (string msg in kv.Value)
                    {
                        builder.Add(kv.Key, msg);
                    }
                }
            }
            else if (r.IsError(out _))
            {
                return r;
            }
        }

        return builder?.ToResult() ?? Robotico.Result.Result.Success();
    }

    /// <summary>
    /// Creates a rule from a predicate and a single field error when the predicate fails.
    /// </summary>
    /// <param name="predicate">Returns true when the rule passes.</param>
    /// <param name="fieldName">Field name for the validation error when the predicate fails.</param>
    /// <param name="errorMessage">Error message when the predicate fails.</param>
    /// <returns>A rule that validates using the predicate.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> or <paramref name="fieldName"/> or <paramref name="errorMessage"/> is null.</exception>
    public static IRule<T> When<T>(Func<T, bool> predicate, string fieldName, string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(fieldName);
        ArgumentNullException.ThrowIfNull(errorMessage);
        return new PredicateRule<T>(predicate, fieldName, errorMessage);
    }

    private sealed class PredicateRule<T> : IRule<T>
    {
        private readonly Func<T, bool> _predicate;
        private readonly string _fieldName;
        private readonly string _errorMessage;

        internal PredicateRule(Func<T, bool> predicate, string fieldName, string errorMessage)
        {
            _predicate = predicate;
            _fieldName = fieldName;
            _errorMessage = errorMessage;
        }

        public Robotico.Result.Result Validate(T instance)
        {
            return _predicate(instance) ? Robotico.Result.Result.Success() : ValidationErrors.ForField(_fieldName, _errorMessage);
        }
    }
}
