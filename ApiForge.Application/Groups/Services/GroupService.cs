using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;
using ApiForge.Application.Groups.Interfaces;
using Microsoft.Extensions.Logging;

namespace ApiForge.Application.Groups.Services;

public sealed class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly ILogger<GroupService> _logger;

    public GroupService(IGroupRepository groupRepository, ILogger<GroupService> logger)
    {
        _groupRepository = groupRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResult<GroupResponse>>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        var (items, totalCount) = await _groupRepository.GetPagedAsync(page, pageSize, ct);

        var responseItems = items.Select(g => new GroupResponse(g.Id, g.Name, g.CreatedAt, g.UpdatedAt)).ToList();

        var pagedResult = new PagedResult<GroupResponse>(responseItems, totalCount, page, pageSize);

        _logger.LogInformation("Retrieved {Count} groups for page {Page}", responseItems.Count, page);

        return Result<PagedResult<GroupResponse>>.Success(pagedResult);
    }
}
