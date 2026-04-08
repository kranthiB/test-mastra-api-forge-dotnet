using ApiForge.Domain.GroupUserAssignments;
using ApiForge.Application.Common.Interfaces;

namespace ApiForge.Application.GroupUserAssignments.Interfaces;

public interface IGroupUserAssignmentRepository : IRepository<GroupUserAssignment>
{
    Task<IReadOnlyList<GroupUserAssignment>> GetAllAsync(Guid? groupId, Guid? userId, CancellationToken ct = default);
    Task<bool> ExistsByCompositeKeyAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default);
}
