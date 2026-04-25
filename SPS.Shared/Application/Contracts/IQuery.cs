using MediatR;

namespace SPS.Shared.Application.Contracts
{
    public interface IQuery<out TResponse> : IRequest<TResponse>;
}
