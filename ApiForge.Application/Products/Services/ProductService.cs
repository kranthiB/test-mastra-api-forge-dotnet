using ApiForge.Application.Common.Models;
using ApiForge.Application.Products.DTOs;
using ApiForge.Application.Products.Interfaces;
using ApiForge.Application.Products.Validators;
using ApiForge.Domain.Products;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ApiForge.Application.Products.Services;

/// <summary>
/// Orchestrates product CRUD operations.
/// Validation → business rules → repository → mapping.
/// </summary>
public sealed class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IValidator<CreateProductRequest> _createValidator;
    private readonly IValidator<UpdateProductRequest> _updateValidator;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository repository,
        IValidator<CreateProductRequest> createValidator,
        IValidator<UpdateProductRequest> updateValidator,
        ILogger<ProductService> logger)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    // ── GET (paged) ────────────────────────────────────────────────────────

    public async Task<Result<PagedResult<ProductResponse>>> GetAllAsync(
        int page, int pageSize, bool? isActive, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 100) pageSize = 10;

        var (items, total) = await _repository.GetPagedAsync(page, pageSize, isActive, ct);
        var responses = items.Select(MapToResponse).ToList();
        return Result<PagedResult<ProductResponse>>.Success(
            new PagedResult<ProductResponse>(responses, total, page, pageSize));
    }

    // ── GET by Id ──────────────────────────────────────────────────────────

    public async Task<Result<ProductResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        return product is null
            ? Result<ProductResponse>.NotFound($"Product '{id}' was not found.")
            : Result<ProductResponse>.Success(MapToResponse(product));
    }

    // ── CREATE ─────────────────────────────────────────────────────────────

    public async Task<Result<ProductResponse>> CreateAsync(
        CreateProductRequest request, CancellationToken ct = default)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result<ProductResponse>.Validation(
                string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage)));

        if (await _repository.ExistsByNameAsync(request.Name, ct: ct))
            return Result<ProductResponse>.Conflict(
                $"A product named '{request.Name}' already exists.");

        var product = Product.Create(
            request.Name, request.Description, request.Price, request.StockQuantity);

        await _repository.AddAsync(product, ct);

        _logger.LogInformation(
            "Product created | Id={ProductId} Name={Name}", product.Id, product.Name);

        return Result<ProductResponse>.Success(MapToResponse(product));
    }

    // ── UPDATE ─────────────────────────────────────────────────────────────

    public async Task<Result<ProductResponse>> UpdateAsync(
        Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result<ProductResponse>.Validation(
                string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage)));

        var product = await _repository.GetByIdAsync(id, ct);
        if (product is null)
            return Result<ProductResponse>.NotFound($"Product '{id}' was not found.");

        if (await _repository.ExistsByNameAsync(request.Name, excludeId: id, ct: ct))
            return Result<ProductResponse>.Conflict(
                $"A product named '{request.Name}' already exists.");

        product.Update(request.Name, request.Description, request.Price, request.StockQuantity);
        await _repository.UpdateAsync(product, ct);

        _logger.LogInformation("Product updated | Id={ProductId}", id);
        return Result<ProductResponse>.Success(MapToResponse(product));
    }

    // ── DELETE ─────────────────────────────────────────────────────────────

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct))
            return Result<bool>.NotFound($"Product '{id}' was not found.");

        await _repository.DeleteAsync(id, ct);
        _logger.LogInformation("Product deleted | Id={ProductId}", id);
        return Result<bool>.Success(true);
    }

    // ── Mapping ────────────────────────────────────────────────────────────

    private static ProductResponse MapToResponse(Product p) => new(
        p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.IsActive, p.CreatedAt, p.UpdatedAt);
}
