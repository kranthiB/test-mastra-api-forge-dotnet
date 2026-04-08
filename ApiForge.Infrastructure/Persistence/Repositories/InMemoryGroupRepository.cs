using ApiForge.Application.Groups.Interfaces;
using ApiForge.Domain.Groups;

namespace ApiForge.Infrastructure.Persistence.Repositories;

public sealed class InMemoryGroupRepository : InMemoryRepository<Group>, IGroupRepository
{
    public Task<(IReadOnlyList<Group> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var allItems = Store.Values.OrderByDescending(x => x.CreatedAt).ToList();
        var items = allItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult<(IReadOnlyList<Group>, int)>((items, allItems.Count));
    }

    public Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken ct = default)
    {
        var exists = Store.Values.Any(g =>
            string.Equals(g.Name, name, StringComparison.OrdinalIgnoreCase) &&
            (!excludeId.HasValue || g.Id != excludeId.Value));
        return Task.FromResult(exists);
    }
}
