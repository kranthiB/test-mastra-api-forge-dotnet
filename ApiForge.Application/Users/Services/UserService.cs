
using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using ApiForge.Domain.Users;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ApiForge.Application.Users.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateUserRequest> _createValidator;
    private readonly IValidator<ReplaceUserRequest> _updateValidator;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IValidator<CreateUserRequest> createValidator,
        IValidator<ReplaceUserRequest> updateValidator,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken ct)
    {
        var validationResult = await _createValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            // In a real app, you might want to concatenate all errors
            return Result<UserResponse>.Validation(validationResult.Errors.First().ErrorMessage);
        }

        var emailExists = await _userRepository.ExistsByEmailAsync(request.Email, null, ct);
        if (emailExists)
        {
            return Result<UserResponse>.Conflict($"A user with email '{request.Email}' already exists.");
        }

        var user = User.Create(request.UserName, request.Email, null); // Assuming CNAME is not provided on creation

        await _userRepository.AddAsync(user, ct);

        _logger.LogInformation("User {UserId} created successfully", user.Id);

        var response = new UserResponse(
            user.Id,
            user.UserName,
            user.Email,
            user.Cname,
            user.CreatedAt,
            user.UpdatedAt);

        return Result<UserResponse>.Success(response);
    }

    public async Task<Result<UserResponse>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null)
        {
            return Result<UserResponse>.NotFound($"User with ID '{id}' not found.");
        }

        var response = new UserResponse(
            user.Id,
            user.UserName,
            user.Email,
            user.Cname,
            user.CreatedAt,
            user.UpdatedAt);

        return Result<UserResponse>.Success(response);
    }

    public async Task<Result<OffsetPagedResult<UserResponse>>> GetAllAsync(int offset, int limit, CancellationToken ct)
    {
        var (users, totalCount) = await _userRepository.GetPagedAsync(offset, limit, ct);

        var userResponses = users.Select(user => new UserResponse(
            user.Id,
            user.UserName,
            user.Email,
            user.Cname,
            user.CreatedAt,
            user.UpdatedAt
        )).ToList();

        var pagedResult = new OffsetPagedResult<UserResponse>(userResponses, totalCount, offset, limit);
        
        _logger.LogInformation("Retrieved {UserCount} of {TotalUsers} users", users.Count, totalCount);

        return Result<OffsetPagedResult<UserResponse>>.Success(pagedResult);
    }

    public async Task<Result<UserResponse>> UpdateAsync(Guid id, ReplaceUserRequest request, CancellationToken ct)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return Result<UserResponse>.Validation(validationResult.Errors.First().ErrorMessage);
        }

        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user is null)
        {
            return Result<UserResponse>.NotFound($"User with ID '{id}' not found.");
        }

        var emailExists = await _userRepository.ExistsByEmailAsync(request.Email, id, ct);
        if (emailExists)
        {
            return Result<UserResponse>.Conflict($"A user with email '{request.Email}' already exists.");
        }

        user.Update(request.UserName, request.Email, user.Cname); // CName is not part of the update request

        await _userRepository.UpdateAsync(user, ct);

        _logger.LogInformation("User {UserId} updated successfully", user.Id);

        var response = new UserResponse(
            user.Id,
            user.UserName,
            user.Email,
            user.Cname,
            user.CreatedAt,
            user.UpdatedAt);

        return Result<UserResponse>.Success(response);
    }

    public async Task<Result<object?>> DeleteAsync(Guid id, CancellationToken ct)
    {
        var userExists = await _userRepository.ExistsAsync(id, ct);
        if (!userExists)
        {
            return Result<object?>.NotFound($"User with ID '{id}' not found.");
        }

        await _userRepository.DeleteAsync(id, ct);

        _logger.LogInformation("User {UserId} deleted successfully", id);

        return Result<object?>.Success(null);
    }
}
