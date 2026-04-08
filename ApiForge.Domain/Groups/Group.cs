
using System.Text.RegularExpressions;
using ApiForge.Domain.Common;

namespace ApiForge.Domain.Groups;

public sealed class Group : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string GroupSlug { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private Group() { }

    public static Group Create(string name, string? description = null)
    {
        // TODO: Add domain validation
        return new Group
        {
            Name = name,
            Description = description,
            GroupSlug = GenerateSlug(name)
        };
    }

    public void Update(string name, string? description = null)
    {
        // TODO: Add domain validation
        Name = name;
        Description = description;
        GroupSlug = GenerateSlug(name);
        Touch();
    }

    private static string GenerateSlug(string phrase)
    {
        var s = phrase.ToLowerInvariant();
        s = Regex.Replace(s, @"[^a-z0-9\s-]", "");
        s = Regex.Replace(s, @"\s+", " ").Trim();
        s = Regex.Replace(s, @"\s", "-");
        return s;
    }
}
