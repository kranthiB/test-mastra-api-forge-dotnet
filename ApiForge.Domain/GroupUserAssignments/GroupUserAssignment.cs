using ApiForge.Domain.Common;

namespace ApiForge.Domain.GroupUserAssignments;

public sealed class GroupUserAssignment : AuditableEntity
{
    public Guid GroupId { get; private set; }
    public string GroupSlug { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }

    private GroupUserAssignment() { } // For EF Core

    public static GroupUserAssignment Create(Guid groupId, string groupSlug, Guid userId)
    {
        var assignment = new GroupUserAssignment
        {
            GroupId = groupId,
            GroupSlug = groupSlug,
            UserId = userId
        };
        return assignment;
    }
}
