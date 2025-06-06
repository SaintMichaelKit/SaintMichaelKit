# SaintMichaelKit.LiteMediator

Lightweight and modular **Mediator** implementation for in-process messaging in .NET, focusing on simplicity, extensibility, and performance.

| Package                 | Version                                                                                                | Downloads |
|-------------------------|--------------------------------------------------------------------------------------------------------| ----- |
| `SaintMichaelKit.LiteMediator` | [![NuGet](https://img.shields.io/nuget/v/SaintMichaelKit.LiteMediator.svg)](https://nuget.org/packages/SaintMichaelKit.LiteMediator) | [![Nuget](https://img.shields.io/nuget/dt/SaintMichaelKit.LiteMediator.svg)](https://nuget.org/packages/SaintMichaelKit.LiteMediator) |

---

## Features

- ✅ Simple in-process mediator pattern implementation.
- ✅ Support for **request/response** and **notification** messaging.
- ✅ Automatic registration of handlers via assembly scanning.
- ✅ Support for **pipeline behaviors** for cross-cutting concerns (validation, logging, exception handling, etc.).

---

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package SaintMichaelKit.LiteMediator --version 1.*
```
Or via Package Manager Console:

```bash
Install-Package SaintMichaelKit.LiteMediator -Version 1.*
```

---

## Usage

### Register the Mediator and behaviors in your DI container:

```csharp
services.AddSimpleMediator();
services.AddMediatorBehavior<ValidationPipelineBehavior<,>>();
services.AddMediatorBehavior<ExceptionHandlingPipelineBehavior<,>>();
services.AddMediatorBehavior<RequestLoggingPipelineBehavior<,>>();
```

### Define your requests and handlers:

```csharp
public class MyRequest : IRequest<MyResponse>
{
    public int Value { get; set; }
}

public class MyResponse
{
    public string Message { get; set; }
}

public class MyRequestHandler : IRequestHandler<MyRequest, MyResponse>
{
    public async Task<MyResponse> Handle(MyRequest request, CancellationToken cancellationToken)
    {
        return new MyResponse { Message = $"Processed {request.Value}" };
    }
}
```

### Sending requests and publishing notifications:

```csharp
var mediator = serviceProvider.GetRequiredService<IMediator>();

// Send a request and get response
var response = await mediator.Send(new MyRequest { Value = 42 });

// Publish a notification
await mediator.Publish(new MyNotification { /* ... */ });
```

---

## Documentation
Check out the [GitHub repository](https://github.com/SaintMichaelKit/SaintMichaelKit) for full documentation, examples, and source code.

## Changelog
See the [CHANGELOG.md](https://github.com/SaintMichaelKit/SaintMichaelKit/blob/main/src/SaintMichaelKit.LiteMediator/CHANGELOG.md) for details on new features, bug fixes, and version history.

## License
This project is licensed under the MIT License. See the [LICENSE](https://github.com/SaintMichaelKit/SaintMichaelKit/blob/main/src/SaintMichaelKit.LiteMediator/LICENSE) file for details.