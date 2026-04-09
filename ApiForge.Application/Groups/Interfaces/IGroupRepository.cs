
using ApiForge.Application.Common.Interfaces;
using ApiForge.Domain.Groups;

namespace ApiForge.Application.Groups.Interfaces;

public interface IGroupRepository : IRepository<Group>
{
    Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
