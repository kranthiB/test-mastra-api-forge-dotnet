
using System.Text.Json.Serialization;

namespace ApiForge.Application.Users.DTOs;

public sealed record UserResponse
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Cname { get; init; }

    [JsonPropertyName("_links")]
    public Dictionary<string, object> Links { get; init; } = new();
}
