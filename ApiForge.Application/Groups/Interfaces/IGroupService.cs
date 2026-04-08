using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;

namespace ApiForge.Application.Groups.Interfaces;

public interface IGroupService
{
    Task<Result<PagedResult<GroupResponse>>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Result<GroupResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<GroupResponse>> GetBySlugAsync(string groupSlug, CancellationToken ct = default);
    Task<Result<GroupResponse>> CreateAsync(CreateGroupRequest request, CancellationToken ct = default);
    Task<Result<GroupResponse>> UpdateAsync(string groupSlug, UpdateGroupRequest request, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result<bool>> DeleteBySlugAsync(string groupSlug, CancellationToken ct = default);
}
