using ApiForge.Application.GroupUserAssignments.Interfaces;
using ApiForge.Domain.GroupUserAssignments;
using ApiForge.Infrastructure.Persistence.Repositories;

namespace ApiForge.Infrastructure.Persistence.Repositories;

public sealed class InMemoryGroupUserAssignmentRepository : InMemoryRepository<GroupUserAssignment>, IGroupUserAssignmentRepository
{
    public Task<GroupUserAssignment?> GetByGroupAndUserAsync(Guid groupId, Guid userId, CancellationToken ct = default)
    {
        var assignment = Store.Values.FirstOrDefault(x => x.GroupId == groupId && x.UserId == userId);
        return Task.FromResult(assignment);
    }

    public Task<bool> ExistsByGroupAndUserAsync(Guid groupId, Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult(Store.Values.Any(x => x.GroupId == groupId && x.UserId == userId));
    }
}
