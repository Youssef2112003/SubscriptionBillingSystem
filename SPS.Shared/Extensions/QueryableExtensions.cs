using Microsoft.EntityFrameworkCore;
using SPS.Shared.Response;
using System.Linq.Expressions;

namespace SPS.Shared.Extensions;

public static class QueryableExtensions
{

    public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var totalCount = await source.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = totalCount > 0
            ? await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false)
            : new List<T>();

        return PaginatedResult<T>.Create(items, totalCount, pageNumber, pageSize);
    }


    public static IQueryable<T> ApplyOrdering<T>(
        this IQueryable<T> query,
        string? sortBy,
        bool isAscending,
        IDictionary<string, Expression<Func<T, object>>> sortExpressions)
    {
        if (string.IsNullOrWhiteSpace(sortBy) || !sortExpressions.TryGetValue(sortBy, out var expression))
            return query;

        return isAscending ? query.OrderBy(expression) : query.OrderByDescending(expression);
    }


    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}