
namespace ApiForge.Application.GroupUserAssignments.DTOs;

public sealed record GroupUserAssignmentResponse(
    Guid Id,
    string GroupSlug,
    Guid UserId,
    DateTime CreatedAt
);
