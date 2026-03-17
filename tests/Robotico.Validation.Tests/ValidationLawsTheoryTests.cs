namespace Robotico.Validation.Tests;

/// <summary>
/// Property-style and parameterized tests for validation laws using [Theory] and [InlineData].
/// Aligns with robotico-results-csharp ResultLawsTheoryTests quality.
/// </summary>
public sealed class ValidationLawsTheoryTests
{
    [Theory]
    [InlineData("a", true)]
    [InlineData("ab", true)]
    [InlineData("", false)]
    public void Rule_When_predicate_satisfied_or_not(string value, bool expectSuccess)
    {
        IRule<string> rule = Rule.When<string>(s => s.Length > 0, "value", "Required.");
        VoidResult r = rule.Validate(value);
        Assert.Equal(expectSuccess, r.IsSuccess());
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(50, true)]
    [InlineData(100, true)]
    [InlineData(-1, false)]
    [InlineData(101, false)]
    public void Rule_ValidateAll_all_rules_pass_or_fail(int value, bool expectSuccess)
    {
        IRule<int> a = Rule.When<int>(i => i >= 0, "n", "Must be non-negative.");
        IRule<int> b = Rule.When<int>(i => i <= 100, "n", "Must be <= 100.");
        VoidResult r = Rule.ValidateAll(value, [a, b]);
        Assert.Equal(expectSuccess, r.IsSuccess());
    }

    [Theory]
    [InlineData("email", "Invalid format.")]
    [InlineData("name", "Required.")]
    [InlineData("code", "Too short.")]
    public void ForField_single_message_produces_validation_error_with_field_and_message(string fieldName, string message)
    {
        VoidResult r = ValidationErrors.ForField(fieldName, message);
        Assert.False(r.IsSuccess());
        Assert.True(r.IsError(out IError? err));
        ValidationError ve = Assert.IsType<ValidationError>(err);
        Assert.Single(ve.Errors);
        Assert.True(ve.Errors.ContainsKey(fieldName));
        Assert.Single(ve.Errors[fieldName]);
        Assert.Equal(message, ve.Errors[fieldName][0]);
    }

    [Fact]
    public void Builder_ToResult_success_when_no_errors_added()
    {
        ValidationErrorsBuilder builder = ValidationErrors.CreateBuilder();
        VoidResult r = builder.ToResult();
        Assert.True(r.IsSuccess());
        Assert.False(builder.HasErrors);
    }

    [Theory]
    [InlineData("x", "e1")]
    [InlineData("field", "Single message")]
    public void Builder_Add_one_field_ToResult_contains_that_field(string fieldName, string errorMessage)
    {
        ValidationErrorsBuilder builder = ValidationErrors.CreateBuilder();
        builder.Add(fieldName, errorMessage);
        Assert.True(builder.HasErrors);
        VoidResult r = builder.ToResult();
        Assert.False(r.IsSuccess());
        Assert.True(r.IsError(out IError? err));
        ValidationError ve = Assert.IsType<ValidationError>(err);
        Assert.Single(ve.Errors[fieldName]);
        Assert.Equal(errorMessage, ve.Errors[fieldName][0]);
    }
}
