using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;

namespace ApiForge.Application.Groups.Interfaces;

public interface IGroupService
{
    Task<Result<PagedResult<GroupResponse>>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
}
