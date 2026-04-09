namespace ApiForge.Application.Users.DTOs;

public record UserResponse(
    Guid Id,
    string UserName,
    string Email,
    string? Cname,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
