namespace ApiForge.Application.Users.DTOs;

public record CreateUserRequest(string UserName, string Email, string Cname, string? DisplayName);
