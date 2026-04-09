namespace ApiForge.Application.Groups.DTOs;

public record GroupResponse(Guid Id, string GroupName, string GroupSlug, string GroupDesc, string? Cname, DateTime CreatedAt, DateTime? UpdatedAt);
