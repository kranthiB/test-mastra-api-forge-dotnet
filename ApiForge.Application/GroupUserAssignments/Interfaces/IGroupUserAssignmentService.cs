
using ApiForge.Application.Common.Models;
using ApiForge.Application.GroupUserAssignments.DTOs;

namespace ApiForge.Application.GroupUserAssignments.Interfaces;

public interface IGroupUserAssignmentService
{
    Task<Result<GroupUserAssignmentResponse>> GetByIdAsync(Guid groupUserAssignId, CancellationToken cancellationToken);

    Task<Result<PagedResult<GroupUserAssignmentResponse>>> GetPaginatedAsync(
        int offset, 
        int limit, 
        string? groupSlug, 
        Guid? userId, 
        CancellationToken cancellationToken = default);

    Task<Result<GroupUserAssignmentResponse>> CreateAsync(CreateGroupUserAssignmentRequest request, CancellationToken cancellationToken);

    Task<Result<bool>> DeleteAsync(Guid groupUserAssignId, CancellationToken cancellationToken);
}
