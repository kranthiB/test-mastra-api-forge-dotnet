
namespace ApiForge.Application.GroupUserAssignments.DTOs;

public sealed record CreateGroupUserAssignmentRequest(string GroupSlug, Guid UserId);
