
using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;

namespace ApiForge.Application.Users.Interfaces;

public interface IUserService
{
    Task<Result<UserResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<OffsetPagedResult<UserResponse>>> GetAllAsync(int offset, int limit, CancellationToken ct = default);
    Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<Result<UserResponse>> UpdateAsync(Guid id, ReplaceUserRequest request, CancellationToken ct = default);
    Task<Result<object?>> DeleteAsync(Guid id, CancellationToken ct = default);
}
