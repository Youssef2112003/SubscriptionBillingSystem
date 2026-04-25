using MediatR;

namespace SPS.Shared.Application.Contracts
{
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
       where TCommand : ICommand<TResponse>;
}
