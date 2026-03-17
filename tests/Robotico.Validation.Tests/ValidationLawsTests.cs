namespace Robotico.Validation.Tests;

/// <summary>
/// Law-style tests: ValidationErrors and Builder obey expected invariants (additive builder, round-trip, success when no errors).
/// </summary>
public sealed class ValidationLawsTests
{
    [Fact]
    public void Builder_ToResult_success_when_no_errors_added()
    {
        ValidationErrorsBuilder builder = ValidationErrors.CreateBuilder();
        VoidResult r = builder.ToResult();
        Assert.True(r.IsSuccess());
        Assert.False(builder.HasErrors);
    }

    [Fact]
    public void Builder_ToResult_failure_when_any_errors_added()
    {
        ValidationErrorsBuilder builder = ValidationErrors.CreateBuilder();
        builder.Add("field", "msg");
        Assert.True(builder.HasErrors);
        VoidResult r = builder.ToResult();
        Assert.False(r.IsSuccess());
        Assert.True(r.IsError(out IError? err));
        ValidationError ve = Assert.IsType<ValidationError>(err);
        Assert.Single(ve.Errors["field"]);
        Assert.Equal("msg", ve.Errors["field"][0]);
    }

    [Fact]
    public void Builder_ToDictionary_Then_ToResult_produces_same_validation_error_shape()
    {
        ValidationErrorsBuilder builder = ValidationErrors.CreateBuilder();
        builder.Add("a", "1").Add("a", "2").Add("b", "3");
        IReadOnlyDictionary<string, string[]> dict = builder.ToDictionary();
        VoidResult r = ValidationErrors.ToResult(dict, "Summary", "CODE");
        Assert.False(r.IsSuccess());
        r.IsError(out IError? err);
        ValidationError ve = Assert.IsType<ValidationError>(err);
        Assert.Equal(2, ve.Errors.Count);
        Assert.Equal(2, ve.Errors["a"].Length);
        Assert.Single(ve.Errors["b"]);
        Assert.Equal("Summary", ve.Message);
        Assert.Equal("CODE", ve.Code);
    }

    [Fact]
    public void ForField_single_message_round_trip_to_ValidationError()
    {
        VoidResult r = ValidationErrors.ForField("email", "Invalid.");
        Assert.False(r.IsSuccess());
        r.IsError(out IError? err);
        ValidationError ve = Assert.IsType<ValidationError>(err);
        Assert.Single(ve.Errors);
        Assert.Single(ve.Errors["email"]);
        Assert.Equal("Invalid.", ve.Errors["email"][0]);
    }

    [Fact]
    public void Builder_Add_is_additive_multiple_fields()
    {
        ValidationErrorsBuilder builder = ValidationErrors.CreateBuilder();
        builder.Add("x", "e1").Add("y", "e2").Add("x", "e3");
        IReadOnlyDictionary<string, string[]> dict = builder.ToDictionary();
        Assert.Equal(2, dict.Count);
        Assert.Equal(2, dict["x"].Length);
        Assert.Single(dict["y"]);
        Assert.Contains("e1", dict["x"]);
        Assert.Contains("e3", dict["x"]);
        Assert.Equal("e2", dict["y"][0]);
    }

    [Fact]
    public void ToResult_with_empty_dictionary_throws_ArgumentException()
    {
        IReadOnlyDictionary<string, string[]> empty = new Dictionary<string, string[]>();
        ArgumentException ex = Assert.Throws<ArgumentException>(() => ValidationErrors.ToResult(empty, null, "VAL_FAILED"));
        Assert.Equal("errors", ex.ParamName);
        Assert.Contains("at least one", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
