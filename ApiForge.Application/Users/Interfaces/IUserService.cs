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
    Task<Result<UserResponse>> CreateAsync(
        CreateUserRequest request,
        CancellationToken ct = default);
}
