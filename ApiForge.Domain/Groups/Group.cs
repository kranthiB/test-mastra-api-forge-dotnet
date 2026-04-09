
using ApiForge.Domain.Common;

namespace ApiForge.Domain.Groups;

public sealed class Group : AuditableEntity
{
    public string GroupName { get; private set; } = string.Empty;
    public string GroupSlug { get; private set; } = string.Empty;
    public string GroupDesc { get; private set; } = string.Empty;
    public string? Cname { get; private set; }

    private Group() { } // For EF Core

    public static Group Create(string groupName, string groupSlug, string groupDesc)
    {
        var group = new Group
        {
            GroupName = groupName,
            GroupSlug = groupSlug,
            GroupDesc = groupDesc
        };
        return group;
    }

    public void Update(string groupName, string groupDesc)
    {
        GroupName = groupName;
        GroupDesc = groupDesc;
        Touch();
    }
}
