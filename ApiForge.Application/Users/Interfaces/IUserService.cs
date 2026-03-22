using ApiForge.Application.Common.Models;

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
    /// Corresponds to operationId: Users_List.
    /// </summary>
    Task<Result<PagedResult<DTOs.UserResponse>>> GetAllAsync(
        int page,
        int pageSize,
        CancellationToken ct = default);
}
