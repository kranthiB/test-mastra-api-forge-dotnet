namespace ApiForge.Application.Products.DTOs;

/// <summary>Payload for PUT /api/v1/products/{id}.</summary>
public record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity);
