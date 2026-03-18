using Asp.Versioning;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace ApiForge.Api.Extensions;

/// <summary>
/// Groups all API-layer service registrations into one clean extension method.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Controllers ────────────────────────────────────────────────────
        services.AddControllers();

        // ── FluentValidation auto-validation (fires before controller action) ──
        services.AddFluentValidationAutoValidation();

        // ── API Versioning ─────────────────────────────────────────────────
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion                        = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified      = true;
                options.ReportApiVersions                        = true;
                options.ApiVersionReader                         = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"));
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat           = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        // ── OpenAPI / Swagger ──────────────────────────────────────────────
        services.AddEndpointsApiExplorer();
        services.AddSwaggerDocs();

        // ── CORS ───────────────────────────────────────────────────────────
        services.AddCors(options =>
            options.AddPolicy("AllowAll", p =>
                p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        // ── Health Checks ──────────────────────────────────────────────────
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("API is alive"));
        // Add more: .AddSqlServer(...), .AddRedis(...), etc.

        // ── Response Caching & Compression ────────────────────────────────
        services.AddResponseCaching();
        services.AddResponseCompression();

        return services;
    }

    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapControllers();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            AllowCachingResponses = false,
            ResponseWriter        = HealthResponseWriter.WriteAsync,
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = _ => true,
        });

        return app;
    }
}
