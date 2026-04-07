namespace ApiForge.Application.Users.DTOs;

public record UserResponse(
    Guid UserId,
    string UserName,
    string Email,
    string? DisplayName,
    string Cname,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);
