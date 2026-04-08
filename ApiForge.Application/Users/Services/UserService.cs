
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
}
