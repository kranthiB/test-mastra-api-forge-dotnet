
using ApiForge.Application.Groups.Interfaces;
using ApiForge.Domain.Groups;

namespace ApiForge.Infrastructure.Persistence.Repositories;

public sealed class InMemoryGroupRepository : InMemoryRepository<Group>, IGroupRepository
{
    public Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var exists = Store.Values.Any(g =>
            g.GroupSlug.Equals(slug, StringComparison.OrdinalIgnoreCase) &&
            (!excludeId.HasValue || g.Id != excludeId.Value));
        return Task.FromResult(exists);
    }

    public Task<(IReadOnlyList<Group> Items, int TotalCount)> GetPaginatedAsync(int offset, int limit, CancellationToken cancellationToken = default)
    {
        var all = Store.Values.OrderByDescending(x => x.CreatedAt).ToList();
        var items = all.Skip(offset).Take(limit).ToList();
        return Task.FromResult<(IReadOnlyList<Group>, int)>((items, all.Count));
    }

    public Task<Group?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var group = Store.Values.FirstOrDefault(g => g.GroupSlug.Equals(slug, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(group);
    }

    public Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var exists = Store.Values.Any(g =>
            g.GroupName.Equals(name, StringComparison.OrdinalIgnoreCase) &&
            (!excludeId.HasValue || g.Id != excludeId.Value));
        return Task.FromResult(exists);
    }
}

