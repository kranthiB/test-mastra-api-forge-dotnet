using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;

namespace ApiForge.Application.Users.Interfaces;

public interface IUserService
{
    Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<UserResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> UpdateAsync(Guid id, CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
