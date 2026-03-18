using ApiForge.Application.Common.Interfaces;
using ApiForge.Domain.Products;

namespace ApiForge.Application.Products.Interfaces;

/// <summary>
/// Product-specific repository, extending the generic contract with
/// domain-specific query methods.
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>Returns a page of products, optionally filtered by active status.</summary>
    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        bool? isActive = null,
        CancellationToken ct = default);

    /// <summary>
    /// Returns true if a product with the given name already exists,
    /// optionally excluding a specific product (used for update checks).
    /// </summary>
    Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken ct = default);
}
