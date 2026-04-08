
namespace ApiForge.Application.Common.Models;

/// <summary>
/// Generic offset-based paginated response envelope.
/// </summary>
/// <typeparam name="T">The DTO item type.</typeparam>
public sealed record OffsetPagedResult<T>(
    IReadOnlyList<T> Items,
    long Total,
    int Offset,
    int Limit
);
