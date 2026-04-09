
using System.Text.Json.Serialization;

namespace ApiForge.Application.Common.Models;

public sealed record PaginatedResponseDto<T>(
    int Offset,
    int Limit,
    int Total,
    [property: JsonPropertyName("_links")]
    Dictionary<string, object> Links,
    [property: JsonPropertyName("data")]
    IReadOnlyList<T> Data
);
