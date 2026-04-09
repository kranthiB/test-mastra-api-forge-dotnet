
using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ApiForge.Application.Users.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<UpdateUserRequest> _updateValidator;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IValidator<UpdateUserRequest> updateValidator,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<Result<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<UserResponse>.Validation(validationResult.ToString());
        }

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return Result<UserResponse>.NotFound($"User with ID {id} not found.");
        }

        if (await _userRepository.ExistsByEmailAsync(request.Email, id, cancellationToken))
        {
            return Result<UserResponse>.Conflict($"Email '{request.Email}' is already in use.");
        }

        user.Update(request.UserName, request.Email, user.Cname);

        await _userRepository.UpdateAsync(user, cancellationToken);

        var userResponse = new UserResponse
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Cname = user.Cname,
            Links = new Dictionary<string, object> { { "self", $"/users/{user.Id}" } }
        };

        _logger.LogInformation("Updated user with id {UserId}", id);
        return Result<UserResponse>.Success(userResponse);
    }

    public async Task<Result<UserResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            _logger.LogInformation("User with id {UserId} not found", id);
            return Result<UserResponse>.NotFound("User not found");
        }

        var userResponse = new UserResponse
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Cname = user.Cname,
            Links = new Dictionary<string, object> { { "self", $"/users/{user.Id}" } }
        };

        _logger.LogInformation("Retrieved user with id {UserId}", id);
        return Result<UserResponse>.Success(userResponse);
    }

    public async Task<Result<PaginatedResponseDto<UserResponse>>> GetAllAsync(int offset, int limit, CancellationToken cancellationToken = default)
    {
        if (limit < 1 || limit > 100)
        {
            return Result<PaginatedResponseDto<UserResponse>>.Validation("Limit must be between 1 and 100.");
        }

        var (users, totalCount) = await _userRepository.GetPaginatedAsync(offset, limit, cancellationToken);

        var userResponses = users.Select(user => new UserResponse
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Cname = user.Cname,
            Links = new Dictionary<string, object> { { "self", $"/users/{user.Id}" } }
        }).ToList();

        _logger.LogInformation("Retrieved {Count} users for offset {Offset} and limit {Limit}. Total users: {TotalCount}", userResponses.Count, offset, limit, totalCount);

        // The controller will be responsible for adding pagination links to the main response object.
        var responseDto = new PaginatedResponseDto<UserResponse>(
            Offset: offset,
            Limit: limit,
            Total: totalCount,
            Links: new Dictionary<string, object>(), // To be filled by controller
            Data: userResponses
        );

        return Result<PaginatedResponseDto<UserResponse>>.Success(responseDto);
    }
}
