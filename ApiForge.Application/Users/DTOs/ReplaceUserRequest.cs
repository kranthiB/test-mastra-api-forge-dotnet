
namespace ApiForge.Application.Users.DTOs;

/// <summary>
/// Represents the data needed to replace an existing user.
/// </summary>
/// <param name="UserName">The user's chosen username.</param>
/// <param name="Email">The user's email address.</param>
public sealed record ReplaceUserRequest(string UserName, string Email);
