using ApiForge.Application.Common.Models;
using ApiForge.Application.Products.DTOs;

namespace ApiForge.Application.Products.Interfaces;

/// <summary>
/// Application-level service contract for the Product resource.
/// All methods return a <see cref="Result{T}"/> so callers can pattern-match
/// on success/failure without catching exceptions.
/// </summary>
public interface IProductService
{
    Task<Result<PagedResult<ProductResponse>>> GetAllAsync(
        int page,
        int pageSize,
        bool? isActive,
        CancellationToken ct = default);

    Task<Result<ProductResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<Result<ProductResponse>> CreateAsync(
        CreateProductRequest request,
        CancellationToken ct = default);

    Task<Result<ProductResponse>> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken ct = default);

    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default);
}
