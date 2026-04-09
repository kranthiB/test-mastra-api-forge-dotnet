
using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using ApiForge.Domain.Users;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ApiForge.Application.Users.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IValidator<CreateUserRequest> _createValidator;
    private readonly IValidator<UpdateUserRequest> _updateValidator;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository repository,
        IValidator<CreateUserRequest> createValidator,
        IValidator<UpdateUserRequest> updateValidator,
        ILogger<UserService> logger)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result<UserResponse>.Validation(validationResult.ToString());

        if (await _repository.ExistsByEmailAsync(request.Email, null, ct))
            return Result<UserResponse>.Conflict($"A user with email '{request.Email}' already exists.");

        var user = User.Create(request.UserName, request.Email, request.Cname);
        await _repository.AddAsync(user, ct);

        _logger.LogInformation("User {UserId} created.", user.Id);
        var response = new UserResponse(user.Id, user.UserName, user.Email, user.Cname, user.CreatedAt, user.UpdatedAt);
        return Result<UserResponse>.Success(response);
    }

    public async Task<Result<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result<UserResponse>.Validation(validationResult.ToString());

        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null)
            return Result<UserResponse>.NotFound($"User with ID '{id}' not found.");

        if (await _repository.ExistsByEmailAsync(request.Email, id, ct))
            return Result<UserResponse>.Conflict($"A user with email '{request.Email}' already exists.");

        user.Update(request.UserName, request.Email);
        await _repository.UpdateAsync(user, ct);

        _logger.LogInformation("User {UserId} updated.", user.Id);
        var response = new UserResponse(user.Id, user.UserName, user.Email, user.Cname, user.CreatedAt, user.UpdatedAt);
        return Result<UserResponse>.Success(response);
    }

    public async Task<Result<PagedResult<UserResponse>>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(page, pageSize, ct);
        var responses = items.Select(user => new UserResponse(user.Id, user.UserName, user.Email, user.Cname, user.CreatedAt, user.UpdatedAt)).ToList();
        return Result<PagedResult<UserResponse>>.Success(new PagedResult<UserResponse>(responses, totalCount, page, pageSize));
    }

    public async Task<Result<UserResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null)
            return Result<UserResponse>.NotFound($"User with ID '{id}' not found.");

        var response = new UserResponse(user.Id, user.UserName, user.Email, user.Cname, user.CreatedAt, user.UpdatedAt);
        return Result<UserResponse>.Success(response);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null)
            return Result<bool>.NotFound($"User with ID '{id}' not found.");

        if (await _repository.HasGroupAssignmentsAsync(id, ct))
            return Result<bool>.Conflict($"User with ID '{id}' has active group assignments and cannot be deleted.");

        await _repository.DeleteAsync(id, ct);
        _logger.LogInformation("User {UserId} deleted.", id);
        return Result<bool>.Success(true);
    }
}
