namespace SaintMichaelKit.LiteMediator.Interfaces;

/// <summary>
/// Represents a request that does not return a response.
/// </summary>
public interface IRequest : IBaseRequest { }

/// <summary>
/// Represents a request that expects a response of type TResponse.
/// </summary>
public interface IRequest<TResponse> : IBaseRequest { }

/// <summary>
/// Marker interface for all request types (with or without response).
/// </summary>
public interface IBaseRequest { }