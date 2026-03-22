namespace Robotico.Validation.Tests;

/// <summary>
/// Tests for <see cref="IValidator{T}"/>, <see cref="ValidationErrors"/>, and <see cref="ValidationErrorsBuilder"/>: success/failure, ForField, ToResult, builder behavior, and argument validation.
/// </summary>
public sealed class ValidationTests
{
    [Fact]
    public void Validator_Validate_returns_success_when_valid()
    {
        AlwaysValidValidator validator = new();
        VoidResult r = validator.Validate("x");
        Assert.True(r.IsSuccess());
    }

    [Fact]
    public void Validator_Validate_returns_validation_error_when_invalid()
    {
        FailingValidator validator = new();
        VoidResult r = validator.Validate(string.Empty);
        Assert.False(r.IsSuccess());
        bool isError = r.IsError(out IError? error);
        Assert.True(isError);
        Assert.NotNull(error);
        Assert.IsType<ValidationError>(error);
        ValidationError ve = (ValidationError)error;
        Assert.Single(ve.Errors);
        Assert.True(ve.Errors.ContainsKey("value"));
        Assert.Contains("Value is required.", ve.Errors["value"]);
    }

    [Fact]
    public void ValidationErrors_ForField_returns_failed_result_with_single_message()
    {
        VoidResult r = ValidationErrors.ForField("email", "Invalid format.");
        Assert.False(r.IsSuccess());
        r.IsError(out IError? err);
        ValidationError ve = Assert.IsType<ValidationError>(err);
        Assert.Single(ve.Errors["email"]);
        Assert.Equal("Invalid format.", ve.Errors["email"][0]);
    }

    [Fact]
    public void ValidationErrors_ForField_multiple_messages_returns_failed_result()
    {
        VoidResult r = ValidationErrors.ForField("name", "Required.", "Too short.");
        Assert.False(r.IsSuccess());
        r.IsError(out IError? err);
        ValidationError ve = Assert.IsType<ValidationError>(err);
        Assert.Equal(2, ve.Errors["name"].Length);
        Assert.Contains("Required.", ve.Errors["name"]);
        Assert.Contains("Too short.", ve.Errors["name"]);
    }

    [Fact]
    public void ValidationErrors_ToResult_with_dictionary_returns_failed_result()
    {
        Dictionary<string, string[]> errors = new()
        {
            ["a"] = ["error A"],
            ["b"] = ["error B1", "error B2"]
        };
        VoidResult r = ValidationErrors.ToResult(errors, "Custom message", "CUSTOM");
        Assert.False(r.IsSuccess());
        r.IsError(out IError? err);
        ValidationError ve = Assert.IsType<ValidationError>(err);
        Assert.Equal(2, ve.Errors.Count);
        Assert.Equal("Custom message", ve.Message);
        Assert.Equal("CUSTOM", ve.Code);
    }

    [Fact]
    public void ValidationErrors_Builder_ToResult_success_when_no_errors()
    {
        ValidationErrorsBuilder builder = ValidationErrors.CreateBuilder();
        VoidResult r = builder.ToResult();
        Assert.True(r.IsSuccess());
        Assert.False(builder.HasErrors);
    }

    [Fact]
    public void ValidationErrors_Builder_ToResult_failed_when_errors_added()
    {
        ValidationErrorsBuilder builder = ValidationErrors.CreateBuilder();
        builder.Add("x", "err1").Add("x", "err2").Add("y", "err3");
        Assert.True(builder.HasErrors);
        VoidResult r = builder.ToResult();
        Assert.False(r.IsSuccess());
        r.IsError(out IError? err);
        ValidationError ve = Assert.IsType<ValidationError>(err);
        Assert.Equal(2, ve.Errors.Count);
        Assert.Equal(2, ve.Errors["x"].Length);
        Assert.Single(ve.Errors["y"]);
    }

    [Fact]
    public void ValidationErrors_Builder_ToDictionary_returns_immutable_like_dictionary()
    {
        ValidationErrorsBuilder builder = ValidationErrors.CreateBuilder();
        builder.Add("f", "m1");
        IReadOnlyDictionary<string, string[]> dict = builder.ToDictionary();
        Assert.Single(dict);
        Assert.Equal("m1", dict["f"][0]);
    }

    [Fact]
    public void ValidationErrors_ForField_throws_on_null_fieldName()
    {
        Assert.Throws<ArgumentNullException>(() => ValidationErrors.ForField(null!, "msg"));
    }

    [Fact]
    public void ValidationErrors_ForField_throws_on_null_errorMessages()
    {
        Assert.Throws<ArgumentNullException>(() => ValidationErrors.ForField("f", (string[])null!));
    }

    [Fact]
    public void ValidationErrors_ToResult_throws_on_null_errors()
    {
        Assert.Throws<ArgumentNullException>(() => ValidationErrors.ToResult(null!));
    }

    [Fact]
    public void ValidationErrors_ForField_throws_on_empty_errorMessages()
    {
        Assert.Throws<ArgumentException>(() => ValidationErrors.ForField("f"));
    }

    [Fact]
    public void ValidationErrors_Builder_Add_throws_on_null_fieldName()
    {
        ValidationErrorsBuilder builder = ValidationErrors.CreateBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.Add(null!, "msg"));
    }

    [Fact]
    public void ValidationErrors_Builder_Add_throws_on_null_errorMessage()
    {
        ValidationErrorsBuilder builder = ValidationErrors.CreateBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.Add("field", null!));
    }
}

