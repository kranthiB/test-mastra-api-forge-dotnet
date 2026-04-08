
using ApiForge.Domain.Common;

namespace ApiForge.Domain.GroupUserAssignments;

public sealed class GroupUserAssignment : AuditableEntity
{
    public Guid GroupId { get; private set; }
    public Guid UserId { get; private set; }

    private GroupUserAssignment() { } // Required for EF Core

    public static GroupUserAssignment Create(Guid groupId, Guid userId)
    {
        return new GroupUserAssignment
        {
            GroupId = groupId,
            UserId = userId
        };
    }
}
