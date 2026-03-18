using ApiForge.Api.Extensions;
using ApiForge.Api.Middleware;
using ApiForge.Application;
using ApiForge.Infrastructure;
using Serilog;

// ─────────────────────────────────────────────────────────────────────────────
// Bootstrap logger (captures startup errors before full Serilog is configured)
// ─────────────────────────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting ApiForge host…");

    var builder = WebApplication.CreateBuilder(args);

    // ── Serilog ───────────────────────────────────────────────────────────
    builder.Host.UseSerilog((ctx, services, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "ApiForge")
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        .WriteTo.File(
            path:            "logs/api-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 14));

    // ── Service registrations ─────────────────────────────────────────────
    builder.Services
        .AddApplication()                                        // Application layer
        .AddInfrastructure(builder.Configuration)               // Infrastructure layer
        .AddApiServices(builder.Configuration);                  // API layer

    // ─────────────────────────────────────────────────────────────────────
    var app = builder.Build();
    // ─────────────────────────────────────────────────────────────────────

    // ── Middleware pipeline ───────────────────────────────────────────────
    // Order matters: exception handler must be outermost.
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.000} ms";
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerDocs();       // Swagger UI only in dev
    }

    app.UseHttpsRedirection();
    app.UseResponseCompression();
    app.UseCors("AllowAll");
    app.UseAuthorization();

    // ── Endpoint mapping ──────────────────────────────────────────────────
    app.MapApiEndpoints();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Expose Program class for integration test WebApplicationFactory<T>
public partial class Program { }
