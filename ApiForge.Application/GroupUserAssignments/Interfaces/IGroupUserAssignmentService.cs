using ApiForge.Application.Common.Models;
using ApiForge.Application.GroupUserAssignments.DTOs;

namespace ApiForge.Application.GroupUserAssignments.Interfaces;

public interface IGroupUserAssignmentService
{
    Task<Result<GroupUserAssignmentResponse>> CreateAsync(CreateGroupUserAssignmentRequest request, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result<GroupUserAssignmentResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
}
