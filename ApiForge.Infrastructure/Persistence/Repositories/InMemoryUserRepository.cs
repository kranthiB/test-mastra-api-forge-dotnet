
using ApiForge.Application.Users.Interfaces;
using ApiForge.Domain.Users;

namespace ApiForge.Infrastructure.Persistence.Repositories;

public sealed class InMemoryUserRepository : InMemoryRepository<User>, IUserRepository
{
    public InMemoryUserRepository()
    {
        // Seed some data for testing
        for (int i = 0; i < 50; i++)
        {
            var user = User.Create($"user{i}", $"user{i}@example.com", $"cname{i}");
            Store.TryAdd(user.Id, user);
        }
    }

    public Task<(IReadOnlyList<User> Items, int TotalCount)> GetPaginatedAsync(int offset, int limit, CancellationToken cancellationToken = default)
    {
        var allUsers = Store.Values.OrderBy(u => u.UserName).ToList();
        var totalCount = allUsers.Count;
        var users = allUsers.Skip(offset).Take(limit).ToList();
        return Task.FromResult<(IReadOnlyList<User>, int)>((users, totalCount));
    }

    public Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var exists = Store.Values.Any(x =>
            string.Equals(x.Email, email, StringComparison.OrdinalIgnoreCase) &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
        return Task.FromResult(exists);
    }
}
