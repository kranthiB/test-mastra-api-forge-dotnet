namespace ApiForge.Application.Groups.DTOs;

public record GroupResponse(
    Guid Id,
    string GroupSlug,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);
