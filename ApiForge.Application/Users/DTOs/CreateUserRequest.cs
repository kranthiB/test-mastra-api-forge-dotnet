namespace ApiForge.Application.Users.DTOs;

/// <summary>Payload for POST /api/v1/users.</summary>
public record CreateUserRequest(
    string UserName,
    string Email,
    string? DisplayName);
