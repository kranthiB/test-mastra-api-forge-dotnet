
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
    private readonly IValidator<CreateGroupRequest> _validator;
    private readonly ILogger<GroupService> _logger;

    public GroupService(IGroupRepository groupRepository, IValidator<CreateGroupRequest> validator, ILogger<GroupService> logger)
    {
        _groupRepository = groupRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GroupResponse>> CreateAsync(CreateGroupRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GroupResponse>.Validation(validationResult.ToString());
        }

        var slugExists = await _groupRepository.ExistsBySlugAsync(request.GroupSlug, cancellationToken: cancellationToken);
        if (slugExists)
        {
            return Result<GroupResponse>.Conflict($"A group with slug '{request.GroupSlug}' already exists.");
        }

        var group = Group.Create(request.GroupName, request.GroupSlug, request.GroupDesc);

        await _groupRepository.AddAsync(group, cancellationToken);

        _logger.LogInformation("Group {GroupId} created successfully.", group.Id);

        var response = new GroupResponse(group.Id, group.GroupName, group.GroupSlug, group.GroupDesc, group.Cname, group.CreatedAt, group.UpdatedAt);

        return Result<GroupResponse>.Success(response);
    }
}
