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

    public async Task<Result<OffsetPagedResult<UserResponse>>> GetUsersAsync(int offset, int limit, CancellationToken ct)
    {
        var (users, totalCount) = await _userRepository.GetUsersAsync(offset, limit, ct);

        var userResponses = users.Select(u => new UserResponse(
            u.Id,
            u.UserName,
            u.Email,
            u.Cname,
            u.CreatedAt,
            u.UpdatedAt
        )).ToList();

        var links = CreateHateoasLinks(totalCount, offset, limit);

        var pagedResult = new OffsetPagedResult<UserResponse>(userResponses, totalCount, offset, limit, links);

        _logger.LogInformation("Retrieved {UserCount} of {TotalUsers} users", userResponses.Count, totalCount);

        return Result<OffsetPagedResult<UserResponse>>.Success(pagedResult);
    }

    private static object CreateHateoasLinks(long total, int offset, int limit)
    {
        var links = new Dictionary<string, object> { { "self", new { href = $"/users?offset={offset}&limit={limit}" } } };

        if (offset > 0)
        {
            links["prev"] = new { href = $"/users?offset={Math.Max(0, offset - limit)}&limit={limit}" };
        }

        if (offset + limit < total)
        {
            links["next"] = new { href = $"/users?offset={offset + limit}&limit={limit}" };
        }

        return links;
    }
}
