namespace ApiForge.Application.Groups.DTOs;

public record GroupResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
