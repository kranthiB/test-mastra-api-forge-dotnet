using ApiForge.Application.Users.Interfaces;
using ApiForge.Domain.Users;

namespace ApiForge.Infrastructure.Persistence.Repositories;

/// <summary>
/// In-memory implementation of <see cref="IUserRepository"/>.
/// Inherits generic CRUD from <see cref="InMemoryRepository{T}"/> and
/// adds user-specific query operations.
/// </summary>
public sealed class InMemoryUserRepository
    : InMemoryRepository<User>, IUserRepository
{
    // ── Domain-specific queries ────────────────────────────────────────────

    public Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, CancellationToken ct = default)
    {
        var ordered = Store.Values
            .OrderByDescending(u => u.CreatedAt)
            .ToList();

        var total = ordered.Count;
        var items = ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult<(IReadOnlyList<User>, int)>((items, total));
    }

    public Task<bool> ExistsByEmailAsync(
        string email, Guid? excludeId = null, CancellationToken ct = default)
    {
        var exists = Store.Values.Any(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase) &&
            (!excludeId.HasValue || u.Id != excludeId.Value));

        return Task.FromResult(exists);
    }
}
