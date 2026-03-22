using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;

namespace ApiForge.Application.Users.Interfaces;

/// <summary>
/// Application-level service contract for the User resource.
/// All methods return a <see cref="Result{T}"/> so callers can pattern-match
/// on success/failure without catching exceptions.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Returns a paginated list of users.
    /// Corresponds to operationId: Users_List (GET /api/v1/users).
    /// </summary>
    Task<Result<PagedResult<UserResponse>>> GetUsersAsync(
        int page,
        int pageSize,
        CancellationToken ct = default);
}
