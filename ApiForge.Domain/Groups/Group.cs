using ApiForge.Domain.Common;

namespace ApiForge.Domain.Groups;

public sealed class Group : AuditableEntity
{
    public string GroupName { get; private set; } = string.Empty;
    public string GroupSlug { get; private set; } = string.Empty;
    public string? GroupDesc { get; private set; }

    private Group() { } // Required for EF Core

    public static Group Create(string groupName, string groupSlug, string? groupDesc)
    {
        // TODO: Add validation, domain events, etc.
        return new Group
        {
            GroupName = groupName,
            GroupSlug = groupSlug,
            GroupDesc = groupDesc,
        };
    }

    public void Update(string groupName, string groupSlug, string? groupDesc)
    {
        // TODO: Add validation, domain events, etc.
        GroupName = groupName;
        GroupSlug = groupSlug;
        GroupDesc = groupDesc;
        Touch();
    }
}
