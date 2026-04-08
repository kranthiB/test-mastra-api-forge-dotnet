
using ApiForge.Application.Common.Models;
using ApiForge.Application.GroupUserAssignments.DTOs;

namespace ApiForge.Application.GroupUserAssignments.Interfaces;

public interface IGroupUserAssignmentService
{
    Task<Result<GroupUserAssignmentResponse>> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Result<IReadOnlyList<GroupUserAssignmentResponse>>> GetAllAsync(Guid? groupId, Guid? userId, CancellationToken ct = default);
    Task<Result<GroupUserAssignmentResponse>> CreateAsync(CreateGroupUserAssignmentRequest request, CancellationToken ct);
    Task<Result<object?>> DeleteAsync(Guid id, CancellationToken ct = default);
}
