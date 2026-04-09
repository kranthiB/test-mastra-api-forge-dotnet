using ApiForge.Application.Common.Models;
using ApiForge.Application.GroupUserAssignments.Interfaces;
using ApiForge.Domain.GroupUserAssignments;

namespace ApiForge.Infrastructure.Persistence.Repositories;

public class InMemoryGroupUserAssignmentRepository : InMemoryRepository<GroupUserAssignment>, IGroupUserAssignmentRepository
{
    public Task<PagedResult<GroupUserAssignment>> GetPaginatedAsync(int offset, int limit, string? groupSlug, Guid? userId, CancellationToken cancellationToken)
    {
        var query = Store.Values.AsQueryable();

        if (!string.IsNullOrEmpty(groupSlug))
        {
            query = query.Where(a => a.GroupSlug == groupSlug);
        }

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        var totalCount = query.Count();
        var items = query.OrderByDescending(x => x.CreatedAt).Skip(offset).Take(limit).ToList();

        var result = new PagedResult<GroupUserAssignment>(items, totalCount, offset, limit);
        return Task.FromResult(result);
    }

    public Task<bool> IsGroupEmptyAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var isEmpty = !Store.Values.Any(a => a.GroupId == groupId);
        return Task.FromResult(isEmpty);
    }
}
