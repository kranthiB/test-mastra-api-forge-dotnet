namespace ApiForge.Application.Products.DTOs;

/// <summary>
/// Read model returned by all product endpoints.
/// Using a <c>record</c> gives value equality for free — useful in tests.
/// </summary>
public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
