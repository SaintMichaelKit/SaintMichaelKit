# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),  
and this project adheres to [Semantic Versioning](https://semver.org/).

---

## [1.1.0] - 2025-06-05

### Added
- Support for **pipeline behaviors**, allowing request pre/post-processing and cross-cutting concerns.
- New extension method `AddMediatorBehavior` for easy registration of `IHandlerBehavior<,>` implementations (generic or concrete).
- Behavior pipeline execution in `Mediator.Send()` for both `IRequest` and `IRequest<TResponse>` types.

---

## [1.0.0] - 2025-05-30

### Added
- Initial release with a **lightweight in-process Mediator** implementation.
- Support for basic request/response (`IRequestHandler`) and notification (`INotificationHandler`) message handling.
- Extension method `AddSimpleMediator` for automatic handler registration via reflection.
