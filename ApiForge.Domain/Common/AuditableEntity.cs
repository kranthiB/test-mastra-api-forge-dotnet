namespace ApiForge.Domain.Common;

/// <summary>
/// Extends <see cref="BaseEntity"/> with creation and modification timestamps.
/// All timestamps are stored in UTC.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    /// <summary>Stamps the entity with the current UTC time on modification.</summary>
    protected void Touch() => UpdatedAt = DateTime.UtcNow;
}
