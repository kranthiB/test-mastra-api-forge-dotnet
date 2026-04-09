namespace ApiForge.Application.GroupUserAssignments.DTOs;

public record GroupUserAssignmentResponse(
    Guid GroupUserAssignId,
    string GroupSlug,
    Guid UserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
