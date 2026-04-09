namespace ApiForge.Application.Common.Models;

public sealed record OffsetPagedResult<T>(
    IReadOnlyList<T> Data,
    int Total,
    int Offset,
    int Limit
);
