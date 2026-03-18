using ApiForge.Domain.Common;

namespace ApiForge.Domain.Products;

/// <summary>
/// Product aggregate root.  All state changes go through the domain methods
/// so invariants are always enforced in one place.
/// </summary>
public sealed class Product : AuditableEntity
{
    // ── Private setters: state is mutated only via domain methods ──────────
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Required by ORMs / serialisers – do not use directly
    private Product() { }

    // ── Factory ────────────────────────────────────────────────────────────

    /// <summary>Creates a new, valid <see cref="Product"/> instance.</summary>
    public static Product Create(
        string name,
        string description,
        decimal price,
        int stockQuantity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegative(price);
        ArgumentOutOfRangeException.ThrowIfNegative(stockQuantity);

        return new Product
        {
            Name = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Price = price,
            StockQuantity = stockQuantity,
        };
    }

    // ── Domain methods ─────────────────────────────────────────────────────

    /// <summary>Updates mutable fields and stamps the modification time.</summary>
    public void Update(string name, string description, decimal price, int stockQuantity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegative(price);
        ArgumentOutOfRangeException.ThrowIfNegative(stockQuantity);

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Price = price;
        StockQuantity = stockQuantity;
        Touch();
    }

    /// <summary>Soft-deletes the product by marking it inactive.</summary>
    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    /// <summary>Re-activates a previously deactivated product.</summary>
    public void Activate()
    {
        IsActive = true;
        Touch();
    }
}
