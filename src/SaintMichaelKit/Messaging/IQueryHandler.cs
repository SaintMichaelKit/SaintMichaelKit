using SaintMichaelKit.Commons;
using SaintMichaelKit.LiteMediator.Interfaces;

namespace SaintMichaelKit.Messaging;

public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
