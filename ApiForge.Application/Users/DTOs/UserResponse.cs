namespace ApiForge.Application.Users.DTOs;

/// <summary>
/// Read model returned by all user endpoints.
/// Using a <c>record</c> gives value equality for free — useful in tests.
/// </summary>
public record UserResponse(
    Guid UserId,
    string UserName,
    string Email,
    string? DisplayName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
