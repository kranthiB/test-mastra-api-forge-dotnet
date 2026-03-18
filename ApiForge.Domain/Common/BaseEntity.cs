namespace ApiForge.Domain.Common;

/// <summary>
/// Root base for all domain entities. Every entity has a stable <see cref="Id"/>
/// that is set on construction and never changes.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}
