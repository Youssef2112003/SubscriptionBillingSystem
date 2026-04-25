namespace SPS.Application.Common;

public interface IIdempotentCommand<out TResponse> : ICommand<TResponse>
{
    Guid IdempotencyKey { get; }
}