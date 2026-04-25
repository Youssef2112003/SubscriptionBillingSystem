using MediatR;
namespace SPS.Shared.Application.Contracts
{
    public interface ICommand<out TResponse> : IRequest<TResponse>;
}
