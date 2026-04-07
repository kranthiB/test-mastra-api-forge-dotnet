using System.Text.Json.Serialization;

namespace ApiForge.Application.Common.Models;

public sealed record OffsetPagedResult<T>(
    IReadOnlyList<T> Items,
    long Total,
    int Offset,
    int Limit,
    [property: JsonPropertyName("_links")]
    object Links
);
