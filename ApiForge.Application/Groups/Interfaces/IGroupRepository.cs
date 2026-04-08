using ApiForge.Application.Common.Interfaces;
using ApiForge.Application.Common.Models;
using ApiForge.Domain.Groups;

namespace ApiForge.Application.Groups.Interfaces;

public interface IGroupRepository : IRepository<Group>
{
    Task<(IReadOnlyList<Group> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken ct = default);
}
