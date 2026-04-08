using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;
using ApiForge.Application.Groups.Interfaces;
using ApiForge.Domain.Groups;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ApiForge.Application.Groups.Services;

public sealed class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IValidator<CreateGroupRequest> _createValidator;
    private readonly IValidator<UpdateGroupRequest> _updateValidator;
    private readonly ILogger<GroupService> _logger;

    public GroupService(
        IGroupRepository groupRepository, 
        IValidator<CreateGroupRequest> createValidator,
        IValidator<UpdateGroupRequest> updateValidator,
        ILogger<GroupService> logger)
    {
        _groupRepository = groupRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<Result<GroupResponse>> CreateAsync(CreateGroupRequest request, CancellationToken ct)
    {
        var validationResult = await _createValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return Result<GroupResponse>.Validation(validationResult.ToString());
        }

        if (await _groupRepository.ExistsByNameAsync(request.Name, null, ct))
        {
            return Result<GroupResponse>.Conflict($"A group with the name '{request.Name}' already exists.");
        }

        var group = Group.Create(request.Name, request.Description);

        if (await _groupRepository.ExistsBySlugAsync(group.GroupSlug, null, ct))
        {
            return Result<GroupResponse>.Conflict($"A group with the slug '{group.GroupSlug}' already exists. Please choose a different name.");
        }

        await _groupRepository.AddAsync(group, ct);

        _logger.LogInformation("Successfully created group {GroupId}", group.Id);

        var response = new GroupResponse(group.Id, group.GroupSlug, group.Name, group.Description, group.CreatedAt, group.UpdatedAt);
        return Result<GroupResponse>.Success(response);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct)
    {
        var exists = await _groupRepository.ExistsAsync(id, ct);
        if (!exists)
        {
            return Result<bool>.NotFound($"Group with id '{id}' not found.");
        }

        await _groupRepository.DeleteAsync(id, ct);
        _logger.LogInformation("Successfully deleted group {GroupId}", id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteBySlugAsync(string groupSlug, CancellationToken ct)
    {
        var group = await _groupRepository.GetBySlugAsync(groupSlug, ct);
        if (group == null)
        {
            return Result<bool>.NotFound($"Group with slug '{groupSlug}' not found.");
        }

        await _groupRepository.DeleteAsync(group.Id, ct);
        _logger.LogInformation("Successfully deleted group {GroupId} with slug {GroupSlug}", group.Id, groupSlug);
        return Result<bool>.Success(true);
    }

    public async Task<Result<PagedResult<GroupResponse>>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        var (items, totalCount) = await _groupRepository.GetPagedAsync(page, pageSize, ct);

        var responseItems = items.Select(g => new GroupResponse(g.Id, g.GroupSlug, g.Name, g.Description, g.CreatedAt, g.UpdatedAt)).ToList();

        var pagedResult = new PagedResult<GroupResponse>(responseItems, totalCount, page, pageSize);

        _logger.LogInformation("Retrieved {Count} groups for page {Page}", responseItems.Count, page);

        return Result<PagedResult<GroupResponse>>.Success(pagedResult);
    }

    public async Task<Result<GroupResponse>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var group = await _groupRepository.GetByIdAsync(id, ct);
        if (group == null)
        {
            return Result<GroupResponse>.NotFound($"Group with id '{id}' not found.");
        }

        var response = new GroupResponse(group.Id, group.GroupSlug, group.Name, group.Description, group.CreatedAt, group.UpdatedAt);
        return Result<GroupResponse>.Success(response);
    }

    public async Task<Result<GroupResponse>> GetBySlugAsync(string groupSlug, CancellationToken ct)
    {
        var group = await _groupRepository.GetBySlugAsync(groupSlug, ct);
        if (group == null)
        {
            return Result<GroupResponse>.NotFound($"Group with slug '{groupSlug}' not found.");
        }

        var response = new GroupResponse(group.Id, group.GroupSlug, group.Name, group.Description, group.CreatedAt, group.UpdatedAt);
        return Result<GroupResponse>.Success(response);
    }

    public async Task<Result<GroupResponse>> UpdateAsync(string groupSlug, UpdateGroupRequest request, CancellationToken ct)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return Result<GroupResponse>.Validation(validationResult.ToString());
        }

        var group = await _groupRepository.GetBySlugAsync(groupSlug, ct);
        if (group == null)
        {
            return Result<GroupResponse>.NotFound($"Group with slug '{groupSlug}' not found.");
        }

        if (await _groupRepository.ExistsByNameAsync(request.Name, group.Id, ct))
        {
            return Result<GroupResponse>.Conflict($"A group with the name '{request.Name}' already exists.");
        }

        group.Update(request.Name, request.Description);

        if (await _groupRepository.ExistsBySlugAsync(group.GroupSlug, group.Id, ct))
        {
            return Result<GroupResponse>.Conflict($"Updating the name to '{request.Name}' would create a conflicting slug '{group.GroupSlug}'. Please choose a different name.");
        }

        await _groupRepository.UpdateAsync(group, ct);

        _logger.LogInformation("Successfully updated group {GroupId}", group.Id);

        var response = new GroupResponse(group.Id, group.GroupSlug, group.Name, group.Description, group.CreatedAt, group.UpdatedAt);
        return Result<GroupResponse>.Success(response);
    }
}
