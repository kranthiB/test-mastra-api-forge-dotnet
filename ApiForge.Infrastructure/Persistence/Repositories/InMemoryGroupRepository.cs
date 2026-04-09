using ApiForge.Application.Groups.Interfaces;
using ApiForge.Domain.Groups;

namespace ApiForge.Infrastructure.Persistence.Repositories;

public sealed class InMemoryGroupRepository : InMemoryRepository<Group>, IGroupRepository
{
    public Task<(IReadOnlyList<Group> Items, int TotalCount)> GetPagedAsync(int offset, int limit, CancellationToken ct = default)
    {
        var all = Store.Values.OrderByDescending(x => x.CreatedAt).ToList();
        var items = all.Skip(offset).Take(limit).ToList();
        return Task.FromResult<(IReadOnlyList<Group>, int)>((items, all.Count));
    }

    public Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null, CancellationToken ct = default)
    {
        var exists = Store.Values.Any(x =>
            string.Equals(x.GroupSlug, slug, StringComparison.OrdinalIgnoreCase) &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
        return Task.FromResult(exists);
    }

    public Task<Group?> GetBySlugAsync(string groupSlug, CancellationToken ct = default)
    {
        var group = Store.Values.FirstOrDefault(x =>
            string.Equals(x.GroupSlug, groupSlug, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(group);
    }
}
