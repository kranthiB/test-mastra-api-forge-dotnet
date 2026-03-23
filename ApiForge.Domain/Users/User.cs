using ApiForge.Domain.Common;

namespace ApiForge.Domain.Users;

/// <summary>
/// User aggregate root.  All state changes go through the domain methods
/// so invariants are always enforced in one place.
/// </summary>
public sealed class User : AuditableEntity
{
    // ── Private setters: state is mutated only via domain methods ──────────
    public string UserName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }

    // Required by ORMs / serialisers – do not use directly
    private User() { }

    // ── Factory ────────────────────────────────────────────────────────────

    /// <summary>Creates a new, valid <see cref="User"/> instance.</summary>
    public static User Create(string userName, string email, string? displayName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        return new User
        {
            UserName = userName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            DisplayName = displayName?.Trim(),
        };
    }

    // ── Mutator ────────────────────────────────────────────────────────────

    /// <summary>
    /// Fully replaces all mutable fields of the user (PUT semantics).
    /// Corresponds to operationId: Users_Replace.
    /// </summary>
    public void Replace(string userName, string email, string? displayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        UserName = userName.Trim();
        Email = email.Trim().ToLowerInvariant();
        DisplayName = displayName?.Trim();
        Touch();
    }
}
