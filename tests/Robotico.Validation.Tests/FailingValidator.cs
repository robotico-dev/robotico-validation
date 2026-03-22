namespace Robotico.Validation.Tests;

internal sealed class FailingValidator : IValidator<string>
{
    public VoidResult Validate(string instance)
    {
        if (string.IsNullOrEmpty(instance))
        {
            return ValidationErrors.ForField("value", "Value is required.");
        }

        return VoidResult.Success();
    }
}
