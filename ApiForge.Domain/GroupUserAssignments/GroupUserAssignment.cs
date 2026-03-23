using ApiForge.Domain.Common;

namespace ApiForge.Domain.GroupUserAssignments;

/// <summary>
/// Represents the assignment of a user to a group.
/// Aggregate root for the GroupUserAssignment resource.
/// </summary>
public sealed class GroupUserAssignment : AuditableEntity
{
    public Guid UserId { get; private set; }
    public Guid GroupId { get; private set; }

    // Required by ORMs / serialisers – do not use directly
    private GroupUserAssignment() { }

    // ── Factory ────────────────────────────────────────────────────────────

    /// <summary>Creates a new, valid <see cref="GroupUserAssignment"/> instance.</summary>
    public static GroupUserAssignment Create(Guid userId, Guid groupId)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId must not be empty.", nameof(userId));
        if (groupId == Guid.Empty) throw new ArgumentException("GroupId must not be empty.", nameof(groupId));

        return new GroupUserAssignment
        {
            UserId  = userId,
            GroupId = groupId,
        };
    }
}
