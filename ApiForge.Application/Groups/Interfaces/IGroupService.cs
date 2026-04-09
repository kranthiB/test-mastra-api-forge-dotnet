
using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;

namespace ApiForge.Application.Groups.Interfaces;

public interface IGroupService
{
    Task<Result<GroupResponse>> CreateAsync(CreateGroupRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<GroupResponse>>> GetPaginatedAsync(int offset, int limit, CancellationToken cancellationToken = default);
    Task<Result<GroupResponse>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<GroupResponse>> UpdateAsync(string groupSlug, UpdateGroupRequest request, CancellationToken cancellationToken = default);
}
