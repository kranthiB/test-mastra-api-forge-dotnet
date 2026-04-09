
using ApiForge.Application.GroupUserAssignments.Interfaces;
using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;
using ApiForge.Application.Groups.Interfaces;
using ApiForge.Domain.Groups;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ApiForge.Application.Groups.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupUserAssignmentRepository _groupUserAssignmentRepository;
    private readonly IValidator<CreateGroupRequest> _createValidator;
    private readonly IValidator<UpdateGroupRequest> _updateValidator;
    private readonly ILogger<GroupService> _logger;

    public GroupService(
        IGroupRepository groupRepository,
        IGroupUserAssignmentRepository groupUserAssignmentRepository,
        IValidator<CreateGroupRequest> createValidator,
        IValidator<UpdateGroupRequest> updateValidator,
        ILogger<GroupService> logger)
    {
        _groupRepository = groupRepository;
        _groupUserAssignmentRepository = groupUserAssignmentRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<Result<GroupResponse>> CreateAsync(CreateGroupRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GroupResponse>.Validation(validationResult.ToString());
        }

        var slugExists = await _groupRepository.ExistsBySlugAsync(request.GroupSlug, cancellationToken: cancellationToken);
        if (slugExists)
        {
            return Result<GroupResponse>.Conflict($"A group with slug '{request.GroupSlug}' already exists.");
        }
        
        var nameExists = await _groupRepository.ExistsByNameAsync(request.GroupName, cancellationToken: cancellationToken);
        if (nameExists)
        {
            return Result<GroupResponse>.Conflict($"A group with name '{request.GroupName}' already exists.");
        }

        var group = Group.Create(request.GroupName, request.GroupSlug, request.GroupDesc);

        await _groupRepository.AddAsync(group, cancellationToken);

        _logger.LogInformation("Group {GroupId} created successfully.", group.Id);

        var response = new GroupResponse(group.Id, group.GroupName, group.GroupSlug, group.GroupDesc, group.Cname, group.CreatedAt, group.UpdatedAt);

        return Result<GroupResponse>.Success(response);
    }

    public async Task<Result<GroupResponse>> UpdateAsync(string groupSlug, UpdateGroupRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GroupResponse>.Validation(validationResult.ToString());
        }

        var group = await _groupRepository.GetBySlugAsync(groupSlug, cancellationToken);
        if (group == null)
        {
            return Result<GroupResponse>.NotFound($"A group with slug '{groupSlug}' was not found.");
        }

        var nameExists = await _groupRepository.ExistsByNameAsync(request.GroupName, group.Id, cancellationToken);
        if (nameExists)
        {
            return Result<GroupResponse>.Conflict($"A group with name '{request.GroupName}' already exists.");
        }

        group.Update(request.GroupName, request.GroupDesc);

        await _groupRepository.UpdateAsync(group, cancellationToken);

        _logger.LogInformation("Group {GroupId} updated successfully.", group.Id);

        var response = new GroupResponse(group.Id, group.GroupName, group.GroupSlug, group.GroupDesc, group.Cname, group.CreatedAt, group.UpdatedAt);

        return Result<GroupResponse>.Success(response);
    }

    public async Task<Result<PagedResult<GroupResponse>>> GetPaginatedAsync(int offset, int limit, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _groupRepository.GetPaginatedAsync(offset, limit, cancellationToken);

        var groupResponses = items.Select(group => new GroupResponse(group.Id, group.GroupName, group.GroupSlug, group.GroupDesc, group.Cname, group.CreatedAt, group.UpdatedAt)).ToList();

        var pagedResult = new PagedResult<GroupResponse>(groupResponses, totalCount, offset, limit);

        return Result<PagedResult<GroupResponse>>.Success(pagedResult);
    }

    public async Task<Result<GroupResponse>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var group = await _groupRepository.GetBySlugAsync(slug, cancellationToken);

        if (group == null)
        {
            return Result<GroupResponse>.NotFound($"A group with slug '{slug}' was not found.");
        }

        var response = new GroupResponse(group.Id, group.GroupName, group.GroupSlug, group.GroupDesc, group.Cname, group.CreatedAt, group.UpdatedAt);

        return Result<GroupResponse>.Success(response);
    }

    public async Task<Result<object>> DeleteBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var group = await _groupRepository.GetBySlugAsync(slug, cancellationToken);
        if (group == null)
        { return Result<object>.NotFound($"A group with slug '{slug}' was not found.");
        }

        var isGroupEmpty = await _groupUserAssignmentRepository.IsGroupEmptyAsync(group.Id, cancellationToken);
        if (!isGroupEmpty)
        {
            return Result<object>.Conflict("Cannot delete group with assigned users.");
        }

        await _groupRepository.DeleteAsync(group.Id, cancellationToken);

        _logger.LogInformation("Group {GroupId} deleted successfully.", group.Id);

        return Result<object>.Success(new object());
    }
}

