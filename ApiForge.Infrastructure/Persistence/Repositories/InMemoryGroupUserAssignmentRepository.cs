using ApiForge.Domain.GroupUserAssignments;
using ApiForge.Application.GroupUserAssignments.Interfaces;

namespace ApiForge.Infrastructure.Persistence.Repositories;

public sealed class InMemoryGroupUserAssignmentRepository : InMemoryRepository<GroupUserAssignment>, IGroupUserAssignmentRepository
{
    public Task<IReadOnlyList<GroupUserAssignment>> GetAllAsync(Guid? groupId, Guid? userId, CancellationToken ct = default)
    {
        var query = Store.Values.AsQueryable();

        if (groupId.HasValue)
        {
            query = query.Where(a => a.GroupId == groupId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        return Task.FromResult<IReadOnlyList<GroupUserAssignment>>(query.ToList());
    }

    public Task<bool> ExistsByCompositeKeyAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default)
    {
        var exists = Store.Values.Any(a => a.UserId == userId && a.GroupId == groupId);
        return Task.FromResult(exists);
    }
}
