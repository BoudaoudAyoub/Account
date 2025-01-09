using Account.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
namespace Account.Infrastructure;

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : AggregateRoot<TKey>
{
    protected readonly AccountDbContext _accountDbContext;
    public DbSet<TEntity> DbSet { get; }
    public IUnitOfWork UnitOfWork => _accountDbContext;

    public Repository(AccountDbContext accountDbContext)
    {
        _accountDbContext = accountDbContext;
        DbSet = _accountDbContext.Set<TEntity>();
    }

    public async Task<TEntity> AddSingleAsync(TEntity entity, CancellationToken cancellationToken)
    {
        return (await DbSet.AddAsync(entity, cancellationToken)).Entity;
    }

    public async Task AddRangeAsync(List<TEntity> entities, CancellationToken cancellationToken)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public IQueryable<TEntity> GetAllAsQueryable() => DbSet.AsNoTracking().AsQueryable();

    public async Task<TEntity> GetByIdAsync(TKey keyValue, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(keyValue);
        return await DbSet.FindAsync(keyValues: [keyValue], cancellationToken: cancellationToken) ?? default!;
    }

    public void UpdateSingle(TEntity entity) => DbSet.Update(entity);

    public void UpdateAll(List<TEntity> entities) => DbSet.UpdateRange(entities);

    public void RemoveSingle(TEntity entity) => DbSet.Remove(entity);

    public void RemoveAll(List<TEntity> entities) => DbSet.RemoveRange(entities);

    public async Task<bool> DoesItExist(TKey keyValue, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(keyValue);
        return await DbSet.AnyAsync(x => keyValue.Equals(x.ID), cancellationToken);
    }
}