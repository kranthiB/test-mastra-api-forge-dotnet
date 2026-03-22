using ApiForge.Domain.Common;

namespace ApiForge.Domain.Users;

/// <summary>
/// User aggregate root.
/// All state changes go through domain methods so invariants are enforced in one place.
/// </summary>
public sealed class User : AuditableEntity
{
    public string UserName { get; private set; } = string.Empty;
    public string Email    { get; private set; } = string.Empty;

    // Required by ORMs / serialisers – do not use directly
    private User() { }

    // ── Factory ────────────────────────────────────────────────────────────

    /// <summary>Creates a new, valid <see cref="User"/> instance.</summary>
    public static User Create(string userName, string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        return new User
        {
            UserName = userName.Trim(),
            Email    = email.Trim().ToLowerInvariant(),
        };
    }

    // ── Domain methods ─────────────────────────────────────────────────────

    /// <summary>Updates mutable fields and stamps the modification time.</summary>
    public void Update(string userName, string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        UserName = userName.Trim();
        Email    = email.Trim().ToLowerInvariant();
        Touch();
    }
}
