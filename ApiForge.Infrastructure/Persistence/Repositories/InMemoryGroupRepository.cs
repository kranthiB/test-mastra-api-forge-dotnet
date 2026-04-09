
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
}
