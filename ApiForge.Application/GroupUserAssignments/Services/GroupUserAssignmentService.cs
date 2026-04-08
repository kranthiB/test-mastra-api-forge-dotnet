
using ApiForge.Application.Common.Models;
using ApiForge.Application.GroupUserAssignments.DTOs;
using ApiForge.Application.GroupUserAssignments.Interfaces;
using ApiForge.Application.Groups.Interfaces;
using ApiForge.Application.Users.Interfaces;
using ApiForge.Domain.GroupUserAssignments;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ApiForge.Application.GroupUserAssignments.Services;

public class GroupUserAssignmentService : IGroupUserAssignmentService
{
    private readonly IGroupUserAssignmentRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IValidator<CreateGroupUserAssignmentRequest> _validator;
    private readonly ILogger<GroupUserAssignmentService> _logger;

    public GroupUserAssignmentService(
        IGroupUserAssignmentRepository repository,
        IUserRepository userRepository,
        IGroupRepository groupRepository,
        IValidator<CreateGroupUserAssignmentRequest> validator,
        ILogger<GroupUserAssignmentService> logger)
    {
        _repository = repository;
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GroupUserAssignmentResponse>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var assignment = await _repository.GetByIdAsync(id, ct);
        if (assignment == null)
        {
            return Result<GroupUserAssignmentResponse>.NotFound("Assignment not found.");
        }

        var response = new GroupUserAssignmentResponse(assignment.Id, assignment.GroupId, assignment.UserId, assignment.CreatedAt, assignment.UpdatedAt);
        return Result<GroupUserAssignmentResponse>.Success(response);
    }

    public async Task<Result<IReadOnlyList<GroupUserAssignmentResponse>>> GetAllAsync(Guid? groupId, Guid? userId, CancellationToken ct = default)
    {
        var assignments = await _repository.GetAllAsync(groupId, userId, ct);
        var response = assignments.Select(a => new GroupUserAssignmentResponse(a.Id, a.GroupId, a.UserId, a.CreatedAt, a.UpdatedAt)).ToList();
        return Result<IReadOnlyList<GroupUserAssignmentResponse>>.Success(response);
    }

    public async Task<Result<GroupUserAssignmentResponse>> CreateAsync(CreateGroupUserAssignmentRequest request, CancellationToken ct)
    {
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return Result<GroupUserAssignmentResponse>.Validation(validationResult.ToString());
        }

        if (!await _userRepository.ExistsAsync(request.UserId, ct))
        {
            return Result<GroupUserAssignmentResponse>.NotFound("User not found.");
        }

        if (!await _groupRepository.ExistsAsync(request.GroupId, ct))
        {
            return Result<GroupUserAssignmentResponse>.NotFound("Group not found.");
        }

        if (await _repository.ExistsByCompositeKeyAsync(request.UserId, request.GroupId, ct))
        {
            return Result<GroupUserAssignmentResponse>.Conflict("User is already assigned to this group.");
        }

        var newAssignment = GroupUserAssignment.Create(request.GroupId, request.UserId);
        var addedAssignment = await _repository.AddAsync(newAssignment, ct);

        _logger.LogInformation("User {UserId} assigned to group {GroupId}", request.UserId, request.GroupId);

        var response = new GroupUserAssignmentResponse(addedAssignment.Id, addedAssignment.GroupId, addedAssignment.UserId, addedAssignment.CreatedAt, addedAssignment.UpdatedAt);
        return Result<GroupUserAssignmentResponse>.Success(response);
    }

    public async Task<Result<object?>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var exists = await _repository.ExistsAsync(id, ct);
        if (!exists)
        {
            return Result<object?>.NotFound("Assignment not found.");
        }

        await _repository.DeleteAsync(id, ct);

        _logger.LogInformation("Deleted group user assignment {Id}", id);

        return Result<object?>.Success(null);
    }
}
