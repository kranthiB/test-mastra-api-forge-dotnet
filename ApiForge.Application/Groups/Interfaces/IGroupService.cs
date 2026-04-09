using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;

namespace ApiForge.Application.Groups.Interfaces;

public interface IGroupService
{
    Task<Result<OffsetPagedResult<GroupResponse>>> GetAllAsync(int offset, int limit, CancellationToken ct = default);
    Task<Result<GroupResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<GroupResponse>> CreateAsync(CreateGroupRequest request, CancellationToken ct = default);
    Task<Result<GroupResponse>> GetBySlugAsync(string groupSlug, CancellationToken ct = default);
}
