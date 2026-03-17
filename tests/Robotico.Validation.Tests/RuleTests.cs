namespace Robotico.Validation.Tests;

/// <summary>
/// Tests for <see cref="Rule"/> and <see cref="IRule{T}"/>: When predicate, ValidateAll composition, empty rules, and argument validation.
/// </summary>
public sealed class RuleTests
{
    [Fact]
    public void Rule_When_predicate_true_returns_success()
    {
        IRule<string> rule = Rule.When<string>(s => s.Length > 0, "value", "Required.");
        VoidResult r = rule.Validate("x");
        Assert.True(r.IsSuccess());
    }

    [Fact]
    public void Rule_When_predicate_false_returns_validation_error()
    {
        IRule<string> rule = Rule.When<string>(s => s.Length > 0, "value", "Required.");
        VoidResult r = rule.Validate("");
        Assert.False(r.IsSuccess());
        r.IsError(out IError? err);
        ValidationError ve = Assert.IsType<ValidationError>(err);
        Assert.Single(ve.Errors["value"]);
        Assert.Equal("Required.", ve.Errors["value"][0]);
    }

    [Fact]
    public void Rule_ValidateAll_all_pass_returns_success()
    {
        IRule<int> a = Rule.When<int>(i => i >= 0, "n", "Must be non-negative.");
        IRule<int> b = Rule.When<int>(i => i <= 100, "n", "Must be <= 100.");
        VoidResult r = Rule.ValidateAll(50, [a, b]);
        Assert.True(r.IsSuccess());
    }

    [Fact]
    public void Rule_ValidateAll_one_fails_returns_combined_errors()
    {
        IRule<int> a = Rule.When<int>(i => i >= 0, "n", "Must be non-negative.");
        IRule<int> b = Rule.When<int>(i => i <= 100, "n", "Must be <= 100.");
        VoidResult r = Rule.ValidateAll(-1, [a, b]);
        Assert.False(r.IsSuccess());
        r.IsError(out IError? err);
        ValidationError ve = Assert.IsType<ValidationError>(err);
        Assert.Single(ve.Errors["n"]);
        Assert.Equal("Must be non-negative.", ve.Errors["n"][0]);
    }

    [Fact]
    public void Rule_ValidateAll_multiple_fail_combines_messages()
    {
        IRule<int> a = Rule.When<int>(i => i >= 0, "n", "Must be non-negative.");
        IRule<int> b = Rule.When<int>(i => i <= 100, "n", "Must be <= 100.");
        VoidResult r = Rule.ValidateAll(150, [a, b]);
        Assert.False(r.IsSuccess());
        r.IsError(out IError? err);
        ValidationError ve = Assert.IsType<ValidationError>(err);
        Assert.Single(ve.Errors["n"]);
        Assert.Equal("Must be <= 100.", ve.Errors["n"][0]);
    }

    [Fact]
    public void Rule_ValidateAll_empty_rules_returns_success()
    {
        VoidResult r = Rule.ValidateAll(42, Array.Empty<IRule<int>>());
        Assert.True(r.IsSuccess());
    }

    [Fact]
    public void Rule_ValidateAll_throws_on_null_rules()
    {
        Assert.Throws<ArgumentNullException>(() => Rule.ValidateAll(42, null!));
    }

    [Fact]
    public void Rule_When_throws_on_null_predicate()
    {
        Assert.Throws<ArgumentNullException>(() => Rule.When<int>(null!, "f", "msg"));
    }

    [Fact]
    public void Rule_When_throws_on_null_fieldName()
    {
        Assert.Throws<ArgumentNullException>(() => Rule.When<int>(_ => true, null!, "msg"));
    }

    [Fact]
    public void Rule_When_throws_on_null_errorMessage()
    {
        Assert.Throws<ArgumentNullException>(() => Rule.When<int>(_ => true, "f", null!));
    }
}
