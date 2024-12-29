namespace Account.Domain.Seedwork;
public interface IRepository<TEntity, Tkey> where TEntity : AggregateRoot<Tkey>
{
    Task<TEntity> AddSingleAsync(TEntity entity, CancellationToken cancellationToken);

    Task AddRangeAsync(List<TEntity> entities, CancellationToken cancellationToken);

    IQueryable<TEntity> GetAllAsQueryable();

    Task<TEntity?> GetByIdAsync(Tkey keyValue, CancellationToken cancellationToken);

    void UpdateSingle(TEntity entity);

    void UpdateAll(List<TEntity> entities);

    void RemoveSingle(TEntity entity);

    void RemoveAll(List<TEntity> entities);

    Task<bool> DoesItExist(Tkey keyValue, CancellationToken cancellationToken);

    IUnitOfWork UnitOfWork { get; }
}