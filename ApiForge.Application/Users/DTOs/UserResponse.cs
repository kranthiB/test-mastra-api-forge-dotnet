namespace ApiForge.Application.Users.DTOs;

/// <summary>
/// Read model returned by all user endpoints.
/// Using a <c>record</c> gives value equality for free — useful in tests.
/// </summary>
public record UserResponse(
    Guid     Id,
    string   UserName,
    string   Email,
    string?  DisplayName,
    bool     IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
