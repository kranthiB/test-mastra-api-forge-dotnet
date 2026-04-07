using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;

namespace ApiForge.Application.Users.Interfaces;

public interface IUserService
{
    Task<Result<OffsetPagedResult<UserResponse>>> GetUsersAsync(int offset, int limit, CancellationToken ct = default);
    Task<Result<UserResponse>> GetByIdAsync(Guid userId, CancellationToken ct = default);
    Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
}
