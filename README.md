# Robotico.Validation

Result-based validation for .NET 8 and .NET 10. Defines `IValidator<T>` (Validate returns `Result`). Integrates with **Robotico.Mediator**'s validation pipeline and **Robotico.Result** for validation errors.

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![C#](https://img.shields.io/badge/C%23-12-239120?logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![GitHub Packages](https://img.shields.io/badge/GitHub%20Packages-Robotico.Validation-blue?logo=github)](https://github.com/robotico-dev/robotico-validation-csharp/packages)
[![Build](https://github.com/robotico-dev/robotico-validation-csharp/actions/workflows/publish.yml/badge.svg)](https://github.com/robotico-dev/robotico-validation-csharp/actions/workflows/publish.yml)

## Features

- **IValidator&lt;T&gt;** — Validate(instance) returns `Result`; success or `Result.ValidationError(...)`.
- **ValidationErrors** — Helpers to build field-to-messages dictionaries: `ForField(name, message)`, `ToResult(errors)`, and a **ValidationErrorsBuilder** to collect errors then call `ToResult()`.
- Works with **Robotico.Mediator** — Register validators per request type; `ValidationPipelineBehavior` runs them before handlers.

## Installation

```bash
dotnet add package Robotico.Validation
```

## Quick start

```csharp
using Robotico.Validation;
using VoidResult = Robotico.Result.Result;

// Implement IValidator<T>
public sealed class CreateUserValidator : IValidator<CreateUserRequest>
{
    public VoidResult Validate(CreateUserRequest request)
    {
        ValidationErrorsBuilder errors = ValidationErrors.CreateBuilder();
        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Name", "Name is required.");
        if (request.Age < 0)
            errors.Add("Age", "Age must be non-negative.");
        return errors.ToResult();
    }
}

// Or use ValidationErrors directly
VoidResult r = ValidationErrors.ForField("email", "Invalid format.");
```

## Versioning

We follow [Semantic Versioning](https://semver.org/). Version **1.0.0** is the first stable release. No breaking changes in minor/patch versions.

## Documentation

See this README for usage and examples; the library follows the same quality bar as **Robotico.Result**.

## Building and testing

From the repo root:

```bash
dotnet restore
dotnet build -c Release
dotnet test tests/Robotico.Validation.Tests/Robotico.Validation.Tests.csproj -c Release
```

With coverage:

```bash
dotnet test tests/Robotico.Validation.Tests/Robotico.Validation.Tests.csproj -c Release --collect:"XPlat Code Coverage"
```

Optional CI gate (fail if line coverage below threshold):

```bash
dotnet test tests/Robotico.Validation.Tests/Robotico.Validation.Tests.csproj -c Release --collect:"XPlat Code Coverage" /p:CollectCoverage=true /p:Threshold=90 /p:ThresholdType=line
```

Or open `robotico-validation.slnx` in your IDE and build from there.

## License

See repository license file.
