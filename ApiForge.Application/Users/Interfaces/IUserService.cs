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
    /// <summary>Creates a new user. operationId: Users_Create</summary>
    Task<Result<UserResponse>> CreateAsync(
        CreateUserRequest request,
        CancellationToken ct = default);

    /// <summary>Returns a single user by ID. operationId: Users_GetById</summary>
    Task<Result<UserResponse>> GetByIdAsync(
        Guid userId,
        CancellationToken ct = default);

    /// <summary>Deletes a user by ID. operationId: Users_Delete</summary>
    Task<Result<bool>> DeleteAsync(
        Guid userId,
        CancellationToken ct = default);
}
