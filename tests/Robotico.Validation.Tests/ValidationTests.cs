using Robotico.Validation;
using Xunit;

namespace Robotico.Validation.Tests;

public sealed class AlwaysValidValidator : IValidator<string>
{
    public Robotico.Result.Result Validate(string instance) => Robotico.Result.Result.Success();
}

public sealed class ValidationTests
{
    [Fact]
    public void Validator_Validate_returns_success()
    {
        IValidator<string> validator = new AlwaysValidValidator();
        Robotico.Result.Result r = validator.Validate("x");
        Assert.True(r.IsSuccess());
    }
}
