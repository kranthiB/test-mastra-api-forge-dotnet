
using System.Collections.Concurrent;
using ApiForge.Application.Users.Interfaces;
using ApiForge.Domain.Users;

namespace ApiForge.Infrastructure.Persistence.Repositories;

public sealed class InMemoryUserRepository : InMemoryRepository<User>, IUserRepository
{
    // Seed with some data for demonstration
    public InMemoryUserRepository()
    {
        var user1 = User.Create("Alice", "alice@example.com", "alice_c");
        var user2 = User.Create("Bob", "bob@example.com", "bob_c");
        var user3 = User.Create("Charlie", "charlie@example.com", null);
        Store.TryAdd(user1.Id, user1);
        Store.TryAdd(user2.Id, user2);
        Store.TryAdd(user3.Id, user3);
    }

    public Task<(IReadOnlyList<User> Items, long TotalCount)> GetPagedAsync(int offset, int limit, CancellationToken ct = default)
    {
        var all = Store.Values.OrderByDescending(x => x.CreatedAt).ToList();
        var items = all.Skip(offset).Take(limit).ToList();
        return Task.FromResult<(IReadOnlyList<User>, long)>((items, all.Count));
    }

    public Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null, CancellationToken ct = default)
    {
        var exists = Store.Values.Any(x =>
            string.Equals(x.Email, email, StringComparison.OrdinalIgnoreCase) &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
        return Task.FromResult(exists);
    }
}
