using System.ComponentModel.DataAnnotations;

namespace ApiForge.Application.GroupUserAssignments.DTOs;

public record CreateGroupUserAssignmentRequest(
    [Required] string GroupSlug,
    [Required] Guid UserId
);
