
using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;

namespace ApiForge.Application.Users.Interfaces;

public interface IUserService
{
    Task<Result<PaginatedResponseDto<UserResponse>>> GetAllAsync(int offset, int limit, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result<object>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
