using SaintMichaelKit.Commons;
using SaintMichaelKit.LiteMediator.Interfaces;

namespace SaintMichaelKit.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
