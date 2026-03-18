using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiForge.Api.Extensions;

/// <summary>
/// Writes a structured JSON health-check response compatible with
/// orchestrators (Kubernetes, ECS, etc.).
/// </summary>
public static class HealthResponseWriter
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
    };

    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status    = report.Status.ToString(),
            duration  = report.TotalDuration.TotalMilliseconds,
            timestamp = DateTime.UtcNow,
            checks    = report.Entries.Select(e => new
            {
                name        = e.Key,
                status      = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration    = e.Value.Duration.TotalMilliseconds,
                exception   = e.Value.Exception?.Message,
            }),
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response, _options));
    }
}
