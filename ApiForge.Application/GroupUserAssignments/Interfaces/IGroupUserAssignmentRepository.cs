using ApiForge.Application.Common.Interfaces;
using ApiForge.Application.Common.Models;
using ApiForge.Domain.GroupUserAssignments;

namespace ApiForge.Application.GroupUserAssignments.Interfaces;

public interface IGroupUserAssignmentRepository : IRepository<GroupUserAssignment>
{
    Task<PagedResult<GroupUserAssignment>> GetPaginatedAsync(
        int offset, 
        int limit, 
        string? groupSlug, 
        Guid? userId, 
        CancellationToken cancellationToken = default);
    Task<bool> IsGroupEmptyAsync(Guid groupId, CancellationToken cancellationToken = default);
}
