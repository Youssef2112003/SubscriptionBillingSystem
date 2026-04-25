using MediatR;

namespace SPS.Application.Common
{
    public interface IQuery<out TResponse> : IRequest<TResponse> { }

}
