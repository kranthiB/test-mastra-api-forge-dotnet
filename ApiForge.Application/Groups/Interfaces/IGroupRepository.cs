
using ApiForge.Application.Common.Interfaces;
using ApiForge.Domain.Groups;

namespace ApiForge.Application.Groups.Interfaces;

public interface IGroupRepository : IRepository<Group>
{
    Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Group> Items, int TotalCount)> GetPaginatedAsync(int offset, int limit, CancellationToken cancellationToken = default);
    Task<Group?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
