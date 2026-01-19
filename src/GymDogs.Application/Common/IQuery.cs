using MediatR;

namespace GymDogs.Application.Common;

public interface IQuery<out TResponse> : IRequest<TResponse> { }

public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{ }
