using MediatR;

namespace SPS.Shared.Application.Contracts
{
    public interface IQueryHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : IQuery<TResponse>;
}
