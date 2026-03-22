namespace Robotico.Validation.Tests;

internal sealed class AlwaysValidValidator : IValidator<string>
{
    public VoidResult Validate(string instance) => VoidResult.Success();
}
