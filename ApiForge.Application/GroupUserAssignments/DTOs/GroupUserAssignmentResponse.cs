namespace ApiForge.Application.GroupUserAssignments.DTOs;

public record GroupUserAssignmentResponse(Guid Id, Guid GroupId, Guid UserId, DateTime CreatedAt, DateTime? UpdatedAt);
