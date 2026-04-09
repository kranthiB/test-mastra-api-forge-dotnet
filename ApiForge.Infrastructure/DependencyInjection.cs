using ApiForge.Application.Products.Interfaces;
using ApiForge.Application.Users.Interfaces;
using ApiForge.Application.Groups.Interfaces;
using ApiForge.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiForge.Infrastructure;

/// <summary>
/// Registers all Infrastructure-layer services (repositories, external clients, etc.).
/// Swap concrete implementations here without touching Application or Domain.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Repositories ───────────────────────────────────────────────────
        // Using Singleton so the in-memory store survives across requests.
        // Replace with AddScoped<IProductRepository, EfProductRepository>()
        // when wiring up a real database.
        services.AddSingleton<IProductRepository, InMemoryProductRepository>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IGroupRepository, InMemoryGroupRepository>();

        // ── TODO: Add database context, caching, message bus clients, etc. ─
        // services.AddDbContext<AppDbContext>(o => o.UseSqlServer(configuration.GetConnectionString("Default")));

        return services;
    }
}
