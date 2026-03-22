using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using ApiForge.Domain.Users;
using Microsoft.Extensions.Logging;

namespace ApiForge.Application.Users.Services;

/// <summary>
/// Orchestrates user read operations.
/// Validation → business rules → repository → mapping.
/// </summary>
public sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository repository, ILogger<UserService> logger)
    {
        _repository = repository;
        _logger     = logger;
    }

    // ── GET (paged) ────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a paginated list of users.
    /// Corresponds to operationId: Users_List (GET /api/v1/users).
    /// </summary>
    public async Task<Result<PagedResult<UserResponse>>> GetUsersAsync(
        int page, int pageSize, CancellationToken ct = default)
    {
        if (page < 1)                        page     = 1;
        if (pageSize is < 1 or > 100)        pageSize = 20;

        var (items, total) = await _repository.GetPagedAsync(page, pageSize, ct);
        var responses = items.Select(MapToResponse).ToList();

        _logger.LogInformation(
            "Users listed | Page={Page} PageSize={PageSize} Total={Total}",
            page, pageSize, total);

        return Result<PagedResult<UserResponse>>.Success(
            new PagedResult<UserResponse>(responses, total, page, pageSize));
    }

    // ── Mapping ────────────────────────────────────────────────────────────

    private static UserResponse MapToResponse(User u) => new(
        u.Id, u.UserName, u.Email, u.DisplayName, u.IsActive, u.CreatedAt, u.UpdatedAt);
}
