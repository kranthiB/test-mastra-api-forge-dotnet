
using ApiForge.Application.Users.Interfaces;
using ApiForge.Domain.Users;

namespace ApiForge.Infrastructure.Persistence.Repositories;

public sealed class InMemoryUserRepository : InMemoryRepository<User>, IUserRepository
{
    public Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null, CancellationToken ct = default)
    {
        var exists = Store.Values.Any(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase) &&
            (!excludeId.HasValue || u.Id != excludeId.Value));
        return Task.FromResult(exists);
    }

    public Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var all = Store.Values.OrderByDescending(x => x.CreatedAt).ToList();
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult<(IReadOnlyList<User>, int)>((items, all.Count));
    }

    public Task<bool> HasGroupAssignmentsAsync(Guid userId, CancellationToken ct = default)
    {
        // This is a mock implementation. In a real system, this would check
        // a data store for group assignments linked to the user.
        // For testing the conflict, the service test will mock this method's response.
        return Task.FromResult(false);
    }
}
