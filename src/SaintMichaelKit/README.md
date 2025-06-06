# SaintMichaelKit

Modular utility library for .NET applications, designed to improve **code organization**, **integration**, and **developer productivity** through a set of lightweight and extensible helper components.

| Package                 | Version                                                                                                | Downloads |
|-------------------------|--------------------------------------------------------------------------------------------------------| ----- |
| `SaintMichaelKit` | [![NuGet](https://img.shields.io/nuget/v/SaintMichaelKit.svg)](https://nuget.org/packages/SaintMichaelKit) | [![Nuget](https://img.shields.io/nuget/dt/SaintMichaelKit.svg)](https://nuget.org/packages/SaintMichaelKit) |

---

## Features

- ✅ General-purpose **helper methods** for everyday development.
- ✅ Clean and consistent **Result/Error** pattern for operation outcomes.
- ✅ Built-in **FluentValidation** integration via pipeline behavior.
- ✅ Includes behaviors for:
  - **Validation**
  - **Logging**
  - **Exception handling**
- ✅ Based on **SaintMichaelKit.LiteMediator**, a lightweight in-process mediator.

---

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package SaintMichaelKit --version 1.*
```
Or via Package Manager Console:

```bash
Install-Package SaintMichaelKit -Version 1.*
```

---

## Usage

### Register helpers and mediator configuration in your DI container:

```csharp
services.AddSimpleMediator();
services.AddMediatorBehavior<ValidationPipelineBehavior<,>>();
services.AddMediatorBehavior<ExceptionHandlingPipelineBehavior<,>>();
services.AddMediatorBehavior<RequestLoggingPipelineBehavior<,>>();
```

### Use `Result` and `Error` in your domain or application layer:

```csharp
public async Task<Result> DoSomethingAsync()
{
    if (someConditionFails)
        return Result.Failure(Error.Validation("ERR001", "Invalid input."));

    return Result.Success();
}
```

### Extend with custom validation:

```csharp
public class MyCommandValidator : AbstractValidator<MyCommand>
{
    public MyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
```
When registered, validators will be automatically executed via ValidationPipelineBehavior.

---

## Documentation
Check out the [GitHub repository](https://github.com/SaintMichaelKit/SaintMichaelKit) for full documentation, examples, and source code.

## Changelog
See the [CHANGELOG.md](https://github.com/SaintMichaelKit/SaintMichaelKit/blob/main/src/SaintMichaelKit/CHANGELOG.md) for details on new features, bug fixes, and version history.

## License
This project is licensed under the MIT License. See the [LICENSE](https://github.com/SaintMichaelKit/SaintMichaelKit/blob/main/src/SaintMichaelKit/LICENSE) file for details.