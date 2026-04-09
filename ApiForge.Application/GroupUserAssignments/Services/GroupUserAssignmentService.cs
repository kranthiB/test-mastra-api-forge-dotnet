
using ApiForge.Application.Common.Models;
using ApiForge.Application.GroupUserAssignments.DTOs;
using ApiForge.Application.GroupUserAssignments.Interfaces;
using ApiForge.Domain.GroupUserAssignments;
using Microsoft.Extensions.Logging;

namespace ApiForge.Application.GroupUserAssignments.Services;

public class GroupUserAssignmentService : IGroupUserAssignmentService
{
    private readonly IGroupUserAssignmentRepository _repository;
    private readonly ILogger<GroupUserAssignmentService> _logger;

    public GroupUserAssignmentService(IGroupUserAssignmentRepository repository, ILogger<GroupUserAssignmentService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<GroupUserAssignmentResponse>> GetByIdAsync(Guid groupUserAssignId, CancellationToken cancellationToken)
    {
        var assignment = await _repository.GetByIdAsync(groupUserAssignId, cancellationToken);

        if (assignment == null)
        {
            return Result<GroupUserAssignmentResponse>.NotFound("Group user assignment not found.");
        }

        var response = new GroupUserAssignmentResponse(assignment.Id, assignment.GroupSlug, assignment.UserId, assignment.CreatedAt, assignment.UpdatedAt);
        return Result<GroupUserAssignmentResponse>.Success(response);
    }

    public async Task<Result<PagedResult<GroupUserAssignmentResponse>>> GetPaginatedAsync(int offset, int limit, string? groupSlug, Guid? userId, CancellationToken cancellationToken)
    {
        var result = await _repository.GetPaginatedAsync(offset, limit, groupSlug, userId, cancellationToken);

        var response = new PagedResult<GroupUserAssignmentResponse>(
            result.Items.Select(a => new GroupUserAssignmentResponse(a.Id, a.GroupSlug, a.UserId, a.CreatedAt, a.UpdatedAt)).ToList(),
            result.TotalCount,
            offset,
            limit
        );

        return Result<PagedResult<GroupUserAssignmentResponse>>.Success(response);
    }

    public async Task<Result<GroupUserAssignmentResponse>> CreateAsync(CreateGroupUserAssignmentRequest request, CancellationToken cancellationToken)
    {
        // For now, we'll just create the assignment. In a real app, you'd validate that the group and user exist.
        var assignment = GroupUserAssignment.Create(request.GroupSlug, request.UserId);
        await _repository.AddAsync(assignment, cancellationToken);

        _logger.LogInformation("Created new group user assignment with ID {Id}", assignment.Id);

        var response = new GroupUserAssignmentResponse(assignment.Id, assignment.GroupSlug, assignment.UserId, assignment.CreatedAt, assignment.UpdatedAt);
        return Result<GroupUserAssignmentResponse>.Success(response);
    }

    public async Task<Result<bool>> DeleteAsync(Guid groupUserAssignId, CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsAsync(groupUserAssignId, cancellationToken);
        if (!exists)
        {
            return Result<bool>.NotFound("Group user assignment not found.");
        }

        await _repository.DeleteAsync(groupUserAssignId, cancellationToken);
        _logger.LogInformation("Deleted group user assignment with ID {Id}", groupUserAssignId);

        return Result<bool>.Success(true);
    }
}
