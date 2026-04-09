using ApiForge.Application.Users.Interfaces;
using ApiForge.Domain.Users;

namespace ApiForge.Infrastructure.Persistence.Repositories;

public sealed class InMemoryUserRepository : InMemoryRepository<User>, IUserRepository
{
    public Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var exists = Store.Values.Any(x =>
            string.Equals(x.Email, email, StringComparison.OrdinalIgnoreCase) &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
        return Task.FromResult(exists);
    }
}
