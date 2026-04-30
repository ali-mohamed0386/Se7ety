namespace Se7ety.Api.Repositories.Interfaces;

public interface IRepository<TEntity>
    where TEntity : class
{
    IQueryable<TEntity> Query();
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
