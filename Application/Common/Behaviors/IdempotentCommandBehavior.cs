using MediatR;

namespace SPS.Application.Common.Behaviors;

public class IdempotentCommandBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdempotentCommand<TResponse>
{
    private readonly ICacheService _cache;

    public IdempotentCommandBehavior(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cacheKey = $"Idempotent:{typeof(TRequest).Name}:{request.IdempotencyKey}";
        var cachedResult = await _cache.GetAsync<TResponse>(cacheKey, cancellationToken);
        if (cachedResult is not null)
            return cachedResult;

        var response = await next(cancellationToken);

        await _cache.SetAsync(cacheKey, response, TimeSpan.FromHours(24), cancellationToken);
        return response;
    }
}