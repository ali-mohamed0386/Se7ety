using Microsoft.EntityFrameworkCore;
using Se7ety.Api.Data;
using Se7ety.Api.Domain.Entities;
using Se7ety.Api.Repositories.Interfaces;

namespace Se7ety.Api.Repositories.Implementations;

public sealed class UserRepository : Repository<User>, IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(user => user.Email == email, cancellationToken);
    }
}
