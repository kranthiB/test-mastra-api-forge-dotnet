using ApiForge.Application.Common.Models;
using ApiForge.Application.GroupUserAssignments.DTOs;
using ApiForge.Application.GroupUserAssignments.Interfaces;
using ApiForge.Domain.GroupUserAssignments;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ApiForge.Application.GroupUserAssignments.Services;

public sealed class GroupUserAssignmentService : IGroupUserAssignmentService
{
    private readonly IGroupUserAssignmentRepository _repository;
    private readonly IValidator<CreateGroupUserAssignmentRequest> _validator;
    private readonly ILogger<GroupUserAssignmentService> _logger;

    public GroupUserAssignmentService(
        IGroupUserAssignmentRepository repository,
        IValidator<CreateGroupUserAssignmentRequest> validator,
        ILogger<GroupUserAssignmentService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GroupUserAssignmentResponse>> CreateAsync(CreateGroupUserAssignmentRequest request, CancellationToken ct = default)
    {
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return Result<GroupUserAssignmentResponse>.Validation(validationResult.ToString());
        }

        if (await _repository.ExistsByGroupAndUserAsync(request.GroupId, request.UserId, ct))
        {
            return Result<GroupUserAssignmentResponse>.Conflict($"User {request.UserId} is already assigned to group {request.GroupId}.");
        }

        var assignment = GroupUserAssignment.Create(request.GroupId, request.UserId);
        await _repository.AddAsync(assignment, ct);

        _logger.LogInformation("Created group-user assignment {Id}", assignment.Id);

        var response = new GroupUserAssignmentResponse(assignment.Id, assignment.GroupId, assignment.UserId, assignment.CreatedAt, assignment.UpdatedAt);
        return Result<GroupUserAssignmentResponse>.Success(response);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var assignment = await _repository.GetByIdAsync(id, ct);
        if (assignment is null)
        {
            return Result<bool>.NotFound($"GroupUserAssignment with id {id} not found.");
        }

        await _repository.DeleteAsync(assignment.Id, ct);
        _logger.LogInformation("Deleted group-user assignment {Id}", id);

        return Result<bool>.Success(true);
    }

    public async Task<Result<GroupUserAssignmentResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var assignment = await _repository.GetByIdAsync(id, ct);
        if (assignment is null)
        {
            return Result<GroupUserAssignmentResponse>.NotFound($"GroupUserAssignment with id {id} not found.");
        }

        var response = new GroupUserAssignmentResponse(assignment.Id, assignment.GroupId, assignment.UserId, assignment.CreatedAt, assignment.UpdatedAt);
        return Result<GroupUserAssignmentResponse>.Success(response);
    }
}
