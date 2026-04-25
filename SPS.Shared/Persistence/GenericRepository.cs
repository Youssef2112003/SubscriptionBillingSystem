using Microsoft.EntityFrameworkCore;
using SPS.Shared.Domain;
using SPS.Shared.Extensions;
using SPS.Shared.Response;
using System.Linq.Expressions;

namespace SPS.Shared.Persistence;

public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
    where TId : notnull
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public GenericRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<TEntity>();
    }

    #region Query

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken).AsTask();

    public async Task<TEntity?> GetByIdAsync(TId id, params Expression<Func<TEntity, object>>[] includes)
    {
        var query = _dbSet.AsQueryable();
        foreach (var include in includes)
            query = query.Include(include);
        return await query.FirstOrDefaultAsync(e => e.Id!.Equals(id));
    }

    public async Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Func<IQueryable<TEntity>, IQueryable<TEntity>>[] includes)
    {
        var query = BuildQueryable(predicate, includes);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TResult?> GetFirstOrDefaultAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken = default)
        => await _dbSet.Where(predicate).Select(selector).FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<TEntity>> ListAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate)
        => await _dbSet.Where(predicate).AsNoTracking().ToListAsync();

    public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector)
        => await _dbSet.Where(predicate).Select(selector).ToListAsync();

    public IQueryable<TEntity> GetQueryable(
        Expression<Func<TEntity, bool>>? filter = null,
        bool asNoTracking = true,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;
        if (asNoTracking) query = query.AsNoTracking();
        foreach (var include in includes) query = query.Include(include);
        if (filter != null) query = query.Where(filter);
        if (orderBy != null) query = orderBy(query);
        return query;
    }

    public IQueryable<TEntity> GetQueryable(
        Expression<Func<TEntity, bool>>? filter = null,
        bool asNoTracking = true,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params string[] includePaths)
    {
        IQueryable<TEntity> query = _dbSet;
        if (asNoTracking) query = query.AsNoTracking();
        foreach (var path in includePaths) query = query.Include(path);
        if (filter != null) query = query.Where(filter);
        if (orderBy != null) query = orderBy(query);
        return query;
    }

    public async Task<PaginatedResult<TEntity>> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 10,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = GetQueryable(filter, asNoTracking: true, orderBy, includes);
        return await query.ToPaginatedResultAsync(pageNumber, pageSize, cancellationToken);
    }

    #endregion

    #region Command

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => _dbSet.AddRangeAsync(entities, cancellationToken);

    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public virtual Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    #endregion

    #region Aggregates

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _dbSet.CountAsync(cancellationToken);

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _dbSet.CountAsync(predicate, cancellationToken);

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _dbSet.AnyAsync(predicate, cancellationToken);

    #endregion

    private IQueryable<TEntity> BuildQueryable(
        Expression<Func<TEntity, bool>>? predicate,
        params Func<IQueryable<TEntity>, IQueryable<TEntity>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();
        if (includes != null)
        {
            foreach (var include in includes)
                query = include(query);
        }
        if (predicate != null)
            query = query.Where(predicate);
        return query;
    }
}