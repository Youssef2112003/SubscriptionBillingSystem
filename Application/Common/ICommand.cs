using MediatR;

namespace SPS.Application.Common
{
    public interface ICommand<out TResponse> : IRequest<TResponse> { }

}
