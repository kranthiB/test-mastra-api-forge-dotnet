
using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;
using ApiForge.Application.Groups.Interfaces;
using ApiForge.Domain.Groups;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ApiForge.Application.Groups.Services;

public sealed class GroupService : IGroupService
{
    private readonly IGroupRepository _repository;
    private readonly IValidator<CreateGroupRequest> _validator;
    private readonly ILogger<GroupService> _logger;

    public GroupService(
        IGroupRepository repository,
        IValidator<CreateGroupRequest> validator,
        ILogger<GroupService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GroupResponse>> CreateAsync(CreateGroupRequest request, CancellationToken ct = default)
    {
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return Result<GroupResponse>.Validation(validationResult.ToString());
        }

        var slugConflict = await _repository.ExistsBySlugAsync(request.GroupSlug, null, ct);
        if (slugConflict)
        {
            return Result<GroupResponse>.Conflict($"A group with slug '{request.GroupSlug}' already exists.");
        }

        var group = Group.Create(
            request.GroupName,
            request.GroupSlug,
            request.GroupDesc
        );

        await _repository.AddAsync(group, ct);
        _logger.LogInformation("Created group {GroupId} with slug {GroupSlug}", group.Id, group.GroupSlug);

        var response = new GroupResponse(
            group.Id,
            group.GroupName,
            group.GroupSlug,
            group.GroupDesc,
            group.CreatedAt,
            group.UpdatedAt
        );

        return Result<GroupResponse>.Success(response);
    }

    public async Task<Result<GroupResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var group = await _repository.GetByIdAsync(id, ct);
        if (group == null)
        {
            return Result<GroupResponse>.NotFound($"Group with ID '{id}' not found.");
        }

        var response = new GroupResponse(
            group.Id,
            group.GroupName,
            group.GroupSlug,
            group.GroupDesc,
            group.CreatedAt,
            group.UpdatedAt
        );

        return Result<GroupResponse>.Success(response);
    }

    public async Task<Result<OffsetPagedResult<GroupResponse>>> GetAllAsync(int offset, int limit, CancellationToken ct = default)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(offset, limit, ct);

        var responseItems = items.Select(g => new GroupResponse(
            g.Id,
            g.GroupName,
            g.GroupSlug,
            g.GroupDesc,
            new DateTimeOffset(g.CreatedAt, TimeSpan.Zero),
            g.UpdatedAt.HasValue ? new DateTimeOffset(g.UpdatedAt.Value, TimeSpan.Zero) : null
        )).ToList();

        var pagedResult = new OffsetPagedResult<GroupResponse>(responseItems, totalCount, offset, limit);
        
        _logger.LogInformation("Retrieved {ItemCount} of {TotalCount} groups.", responseItems.Count, totalCount);

        return Result<OffsetPagedResult<GroupResponse>>.Success(pagedResult);
    }

    public async Task<Result<GroupResponse>> GetBySlugAsync(string groupSlug, CancellationToken ct = default)
    {
        var group = await _repository.GetBySlugAsync(groupSlug, ct);
        if (group == null)
        {
            return Result<GroupResponse>.NotFound($"Group with slug '{groupSlug}' not found.");
        }

        var response = new GroupResponse(
            group.Id,
            group.GroupName,
            group.GroupSlug,
            group.GroupDesc,
            group.CreatedAt,
            group.UpdatedAt
        );

        return Result<GroupResponse>.Success(response);
    }
}
