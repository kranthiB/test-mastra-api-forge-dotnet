using ApiForge.Application.Common.Interfaces;
using ApiForge.Domain.Groups;

namespace ApiForge.Application.Groups.Interfaces;

public interface IGroupRepository : IRepository<Group>
{
    Task<(IReadOnlyList<Group> Items, int TotalCount)> GetPagedAsync(int offset, int limit, CancellationToken ct = default);
    Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null, CancellationToken ct = default);
    Task<Group?> GetBySlugAsync(string groupSlug, CancellationToken ct = default);
}
