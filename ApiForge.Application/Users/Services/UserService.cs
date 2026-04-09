
using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using Microsoft.Extensions.Logging;

namespace ApiForge.Application.Users.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
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
