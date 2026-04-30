using Microsoft.EntityFrameworkCore;
using Se7ety.Api.Data;
using Se7ety.Api.Repositories.Interfaces;

namespace Se7ety.Api.Repositories.Implementations;

public class Repository<TEntity>(ApplicationDbContext dbContext) : IRepository<TEntity>
    where TEntity : class
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public IQueryable<TEntity> Query()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
