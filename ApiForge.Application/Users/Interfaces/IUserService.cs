
using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;

namespace ApiForge.Application.Users.Interfaces;

public interface IUserService
{
    Task<Result<OffsetPagedResult<UserResponse>>> GetAllAsync(int offset, int limit, CancellationToken ct = default);
}
