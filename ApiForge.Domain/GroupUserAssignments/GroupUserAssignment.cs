
using ApiForge.Domain.Common;

namespace ApiForge.Domain.GroupUserAssignments;

public sealed class GroupUserAssignment : AuditableEntity
{
    public string GroupSlug { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }

    private GroupUserAssignment() { } // Required for EF Core

    public static GroupUserAssignment Create(string groupSlug, Guid userId)
    {
        // TODO: Add validation guard clauses if needed, though typically handled in Application layer.
        return new GroupUserAssignment
        {
            GroupSlug = groupSlug,
            UserId = userId
        };
    }
}
