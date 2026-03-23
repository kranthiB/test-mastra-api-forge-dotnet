using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using ApiForge.Domain.Users;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ApiForge.Application.Users.Services;

/// <summary>
/// Orchestrates user operations.
/// Validation → business rules → repository → mapping.
/// </summary>
public sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IValidator<CreateUserRequest> _createValidator;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository repository,
        IValidator<CreateUserRequest> createValidator,
        ILogger<UserService> logger)
    {
        _repository = repository;
        _createValidator = createValidator;
        _logger = logger;
    }

    // ── CREATE ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a new user. Corresponds to operationId: Users_Create.
    /// </summary>
    public async Task<Result<UserResponse>> CreateAsync(
        CreateUserRequest request, CancellationToken ct = default)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result<UserResponse>.Validation(
                string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage)));

        if (await _repository.ExistsByUserNameAsync(request.UserName, ct: ct))
            return Result<UserResponse>.Conflict(
                $"A user with userName '{request.UserName}' already exists.");

        if (await _repository.ExistsByEmailAsync(request.Email, ct: ct))
            return Result<UserResponse>.Conflict(
                $"A user with email '{request.Email}' already exists.");

        var user = User.Create(request.UserName, request.Email, request.DisplayName);

        await _repository.AddAsync(user, ct);

        _logger.LogInformation(
            "User created | Id={UserId} UserName={UserName}", user.Id, user.UserName);

        return Result<UserResponse>.Success(MapToResponse(user));
    }

    // ── Mapping ────────────────────────────────────────────────────────────

    private static UserResponse MapToResponse(User u) => new(
        u.Id, u.UserName, u.Email, u.DisplayName, u.CreatedAt);
}
