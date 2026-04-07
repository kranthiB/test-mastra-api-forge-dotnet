using ApiForge.Application.Products.Interfaces;
using ApiForge.Application.Products.Services;
using ApiForge.Application.Products.Validators;
using ApiForge.Application.Users.Interfaces;
using ApiForge.Application.Users.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ApiForge.Application;

/// <summary>
/// Registers all Application-layer services.
/// Called once from the API's <c>Program.cs</c>.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Validators – scanned from this assembly
        services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();

        // Services
        services.AddScoped<IProductService, ProductService>();

        // User services
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
