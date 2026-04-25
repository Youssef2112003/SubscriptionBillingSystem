using SPS.Shared.Domain;
using SPS.Shared.Response;
using System.Linq.Expressions;

namespace SPS.Shared.Persistence;


public interface IGenericRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
    where TId : notnull
{
    #region Query operations

    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(TId id, params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Func<IQueryable<TEntity>, IQueryable<TEntity>>[] includes);


    Task<TResult?> GetFirstOrDefaultAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> ListAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate);

    Task<IReadOnlyList<TResult>> ListAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector);

    IQueryable<TEntity> GetQueryable(
        Expression<Func<TEntity, bool>>? filter = null,
        bool asNoTracking = true,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params Expression<Func<TEntity, object>>[] includes);


    IQueryable<TEntity> GetQueryable(
        Expression<Func<TEntity, bool>>? filter = null,
        bool asNoTracking = true,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params string[] includePaths);


    Task<PaginatedResult<TEntity>> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 10,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);

    #endregion

    #region Write operations


    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);


    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);


    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);


    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    #endregion

    #region Aggregate operations


    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);


    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    #endregion
}