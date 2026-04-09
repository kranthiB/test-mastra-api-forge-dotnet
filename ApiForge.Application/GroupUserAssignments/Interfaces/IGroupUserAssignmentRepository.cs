using ApiForge.Application.Common.Interfaces;
using ApiForge.Domain.GroupUserAssignments;

namespace ApiForge.Application.GroupUserAssignments.Interfaces;

public interface IGroupUserAssignmentRepository : IRepository<GroupUserAssignment>
{
    Task<GroupUserAssignment?> GetByGroupAndUserAsync(Guid groupId, Guid userId, CancellationToken ct = default);
    Task<bool> ExistsByGroupAndUserAsync(Guid groupId, Guid userId, CancellationToken ct = default);
}
