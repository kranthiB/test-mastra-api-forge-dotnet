
using ApiForge.Domain.Common;

namespace ApiForge.Domain.Groups;

public sealed class Group : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private Group() { }

    public static Group Create(string name, string? description = null)
    {
        // TODO: Add domain validation
        return new Group
        {
            Name = name,
            Description = description
        };
    }

    public void Update(string name, string? description = null)
    {
        // TODO: Add domain validation
        Name = name;
        Description = description;
        Touch();
    }
}
