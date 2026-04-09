using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;

namespace ApiForge.Application.Users.Interfaces;

public interface IUserService
{
    Task<Result<PagedResult<UserResponse>>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Result<UserResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<Result<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default);
}
