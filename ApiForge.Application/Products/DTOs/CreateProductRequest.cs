namespace ApiForge.Application.Products.DTOs;

/// <summary>Payload for POST /api/v1/products.</summary>
public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity);
