
namespace ApiForge.Application.Users.DTOs;

public sealed record CreateUserRequest(
    string UserName,
    string Email
);
