namespace ApiForge.Application.Common.Models;

/// <summary>
/// Generic paginated response envelope returned by any list endpoint.
/// </summary>
/// <typeparam name="T">The DTO item type.</typeparam>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
