# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- CI job `trim-validate` — build library with `IsTrimmable` and `EnableTrimAnalyzer`; publish requires trim-validate to pass.
- Guard tests for `ValidationErrors.ForField` (empty error messages → `ArgumentException`) and `ValidationErrors.Builder.Add` (null field name or null error message → `ArgumentNullException`).

## [1.0.0] — Initial release

- `IValidator<T>` with `Result Validate(T instance)`.
- `ValidationErrors.ToResult`, `ValidationErrors.ForField`, `ValidationErrors.CreateBuilder` and `Builder` (Add, HasErrors, ToDictionary, ToResult).
- Integration with Robotico.Result (ValidationError, Result-based API).

[Unreleased]: https://github.com/robotico-dev/robotico-validation-csharp/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/robotico-dev/robotico-validation-csharp/releases/tag/v1.0.0
