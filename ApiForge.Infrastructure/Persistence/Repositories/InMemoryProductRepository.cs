using ApiForge.Application.Products.Interfaces;
using ApiForge.Domain.Products;

namespace ApiForge.Infrastructure.Persistence.Repositories;

/// <summary>
/// In-memory implementation of <see cref="IProductRepository"/>.
/// Inherits generic CRUD from <see cref="InMemoryRepository{T}"/> and
/// adds product-specific query operations.
/// </summary>
public sealed class InMemoryProductRepository
    : InMemoryRepository<Product>, IProductRepository
{
    public InMemoryProductRepository()
    {
        // ── Seed data so the API works out of the box ──────────────────────
        var seed = new[]
        {
            Product.Create("Laptop Pro 15",      "High-performance developer laptop",  2499.99m, 50),
            Product.Create("Mechanical Keyboard", "Tactile switches, RGB backlit",       149.99m, 200),
            Product.Create("4K Monitor 27\"",     "IPS panel, 144 Hz refresh rate",      599.99m, 75),
            Product.Create("USB-C Hub 7-in-1",    "HDMI, USB 3.0, SD card, PD charging",  49.99m, 500),
        };

        foreach (var p in seed)
            Store[p.Id] = p;
    }

    // ── Domain-specific queries ────────────────────────────────────────────

    public Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, bool? isActive = null, CancellationToken ct = default)
    {
        var query = Store.Values.AsEnumerable();

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        var ordered = query.OrderByDescending(p => p.CreatedAt).ToList();
        var total = ordered.Count;
        var items = ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult<(IReadOnlyList<Product>, int)>((items, total));
    }

    public Task<bool> ExistsByNameAsync(
        string name, Guid? excludeId = null, CancellationToken ct = default)
    {
        var exists = Store.Values.Any(p =>
            string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase) &&
            (!excludeId.HasValue || p.Id != excludeId.Value));

        return Task.FromResult(exists);
    }
}
