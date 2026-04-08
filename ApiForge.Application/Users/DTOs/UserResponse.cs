
namespace ApiForge.Application.Users.DTOs;

public sealed record UserResponse(
    Guid Id,
    string UserName,
    string Email,
    string? Cname,
    DateTime CreatedAt,
    DateTime? UpdatedAt
    // Dictionary<string, string> Links // HATEOAS links can be added here
);
