
using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;

namespace ApiForge.Application.Users.Interfaces;

public interface IUserService
{
    Task<Result<PaginatedResponseDto<UserResponse>>> GetAllAsync(int offset, int limit, CancellationToken cancellationToken = default);
}
