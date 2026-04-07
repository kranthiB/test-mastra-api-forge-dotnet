using ApiForge.Application.Users.Interfaces;
using ApiForge.Domain.Users;

namespace ApiForge.Infrastructure.Persistence.Repositories;

public sealed class InMemoryUserRepository : InMemoryRepository<User>, IUserRepository
{
    public Task<(IReadOnlyList<User> Items, int TotalCount)> GetUsersAsync(int offset, int limit, CancellationToken ct = default)
    {
        var allUsers = Store.Values.OrderByDescending(u => u.CreatedAt).ToList();
        var pagedUsers = allUsers.Skip(offset).Take(limit).ToList();
        return Task.FromResult<(IReadOnlyList<User>, int)>((pagedUsers, allUsers.Count));
    }

    public Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null, CancellationToken ct = default)
    {
        var exists = Store.Values.Any(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase) &&
            (!excludeId.HasValue || u.Id != excludeId.Value));
        return Task.FromResult(exists);
    }
}
