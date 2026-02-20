# Agent Guidelines for Exceptionless.RandomData

You are an expert .NET engineer working on Exceptionless.RandomData, a focused utility library for generating random data useful in unit tests and data seeding. The library provides methods for generating random integers, longs, doubles, decimals, booleans, strings, words, sentences, paragraphs, dates, enums, IP addresses, versions, and coordinates. It also includes an `EnumerableExtensions.Random<T>()` extension method to pick a random element from any collection.

**Craftsmanship Mindset**: Every line of code should be intentional, readable, and maintainable. Write code you'd be proud to have reviewed by senior engineers. Prefer simplicity over cleverness. When in doubt, favor explicitness and clarity.

## Repository Overview

Exceptionless.RandomData provides random data generation utilities for .NET applications:

- **Numeric** - `GetInt`, `GetLong`, `GetDouble`, `GetDecimal` with optional min/max ranges
- **Boolean** - `GetBool` with configurable probability (0-100%)
- **String** - `GetString`, `GetAlphaString`, `GetAlphaNumericString` with configurable length and allowed character sets
- **Text** - `GetWord`, `GetWords`, `GetTitleWords`, `GetSentence`, `GetParagraphs` with lorem ipsum-style words and optional HTML output
- **DateTime** - `GetDateTime`, `GetDateTimeOffset`, `GetTimeSpan` with optional start/end ranges
- **Enum** - `GetEnum<T>()` to pick a random enum value (constrained to `struct, Enum`)
- **Network** - `GetIp4Address` for random IPv4 addresses
- **Version** - `GetVersion` for random version strings with optional min/max
- **Coordinate** - `GetCoordinate` for random lat/lng pairs
- **Collection** - `EnumerableExtensions.Random<T>()` to pick a random element from any `IEnumerable<T>`

Design principles: **simplicity**, **thread safety** (uses `Random.Shared`), **cryptographic quality strings** (uses `RandomNumberGenerator`), **modern .NET features** (targeting net8.0/net10.0).

## Quick Start

```bash
# Build
dotnet build Exceptionless.RandomData.slnx

# Test
dotnet run --project test/Exceptionless.RandomData.Tests -f net8.0

# Format code
dotnet format Exceptionless.RandomData.slnx
```

## Project Structure

```text
src
└── Exceptionless.RandomData
    └── RandomData.cs                        # All random data generation + EnumerableExtensions
test
└── Exceptionless.RandomData.Tests
    ├── RandomDataTests.cs                   # Unit tests
    └── Properties
        └── AssemblyInfo.cs                  # Disables test parallelization
```

## Coding Standards

### Style & Formatting

- Follow `.editorconfig` rules (file-scoped namespaces, K&R braces)
- Follow [Microsoft C# conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use `String.`/`Int32.`/`Char.` for static method access per `.editorconfig` `dotnet_style_predefined_type_for_member_access = false`
- Run `dotnet format` to auto-format code
- Match existing file style; minimize diffs
- No code comments unless necessary—code should be self-explanatory

### Code Quality

- Write complete, runnable code—no placeholders, TODOs, or `// existing code...` comments
- Use modern C# features available in **net8.0/net10.0**
- **Nullable reference types** are enabled—annotate nullability correctly, don't suppress warnings without justification
- **ImplicitUsings** are enabled—don't add `using System;`, `using System.Collections.Generic;`, etc.
- Follow SOLID, DRY principles; remove unused code and parameters
- Clear, descriptive naming; prefer explicit over clever

### Modern .NET Idioms

- **Guard APIs**: Use `ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual()`, `ArgumentOutOfRangeException.ThrowIfLessThan()`, `ArgumentOutOfRangeException.ThrowIfGreaterThan()`, `ArgumentNullException.ThrowIfNull()`, `ArgumentException.ThrowIfNullOrEmpty()` instead of manual checks
- **`Random.Shared`**: Use `Random.Shared` instead of `new Random()` for thread-safe random number generation
- **`RandomNumberGenerator.Fill()`**: Use the static method instead of `RandomNumberGenerator.Create()` + disposal
- **Collection expressions**: Use `[...]` syntax for array initialization
- **`Span<T>`**: Use `stackalloc` and span-based APIs to avoid allocations in hot paths
- **Expression-bodied members**: Use for single-expression methods
- **`Math.Clamp`**: Use instead of separate `Math.Min`/`Math.Max` calls
- **Generic constraints**: Use `where T : struct, Enum` instead of runtime `typeof(T).IsEnum` checks
- **Pattern matching**: Use `is null` / `is not null` instead of `== null` / `!= null`

### Exceptions

- Use `ArgumentOutOfRangeException.ThrowIf*` guard APIs at method entry
- Use `ArgumentException` for invalid arguments that don't fit range checks
- Include parameter names via `nameof()` where applicable
- Fail fast: throw exceptions immediately for invalid arguments

## Making Changes

### Before Starting

1. **Gather context**: Read `RandomData.cs` and the test file to understand the full scope
2. **Research patterns**: Find existing usages of the code you're modifying
3. **Understand completely**: Know the problem, side effects, and edge cases before coding
4. **Plan the approach**: Choose the simplest solution that satisfies all requirements

### While Coding

- **Minimize diffs**: Change only what's necessary, preserve formatting and structure
- **Preserve behavior**: Don't break existing functionality or change semantics unintentionally
- **Build incrementally**: Run `dotnet build` after each logical change to catch errors early
- **Test continuously**: Run tests frequently to verify correctness
- **Match style**: Follow the patterns in surrounding code exactly

### Validation

Before marking work complete, verify:

1. **Builds successfully**: `dotnet build Exceptionless.RandomData.slnx` exits with code 0
2. **All tests pass**: `dotnet run --project test/Exceptionless.RandomData.Tests -f net8.0` shows no failures
3. **No new warnings**: Check build output for new compiler warnings (warnings are treated as errors)
4. **API compatibility**: Public API changes are intentional and backward-compatible when possible
5. **Breaking changes flagged**: Clearly identify any breaking changes for review

## Testing

### Framework

- **xUnit v3** with **Microsoft Testing Platform** as the test runner
- Test parallelization is disabled via `Properties/AssemblyInfo.cs`

### Running Tests

```bash
# All tests, both TFMs
dotnet run --project test/Exceptionless.RandomData.Tests -f net8.0
dotnet run --project test/Exceptionless.RandomData.Tests -f net10.0
```

### Note on Namespace Conflict

The test project uses `<RootNamespace>Exceptionless.Tests</RootNamespace>` to avoid a namespace conflict where the xUnit v3 MTP source generator creates a namespace `Exceptionless.RandomData.Tests` that shadows the `Exceptionless.RandomData` class. The test code uses `using Xunit;` and references `RandomData.*` methods directly since the `Exceptionless` namespace is accessible from within `Exceptionless.Tests`.

## Resources

- [README.md](README.md) - Overview and usage examples
- [NuGet Package](https://www.nuget.org/packages/Exceptionless.RandomData/)
