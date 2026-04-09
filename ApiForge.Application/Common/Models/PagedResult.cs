namespace ApiForge.Application.Common.Models;

/// <summary>
/// Generic paginated response envelope returned by any list endpoint.
/// </summary>
/// <typeparam name="T">The DTO item type.</typeparam>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Offset,
    int Limit);
