namespace SaintMichaelKit.LiteMediator.Interfaces;

public interface IRequest : IBaseRequest { }

public interface IRequest<TResponse> : IBaseRequest { }

public interface IBaseRequest { }