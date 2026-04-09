using ApiForge.Application.GroupUserAssignments.Interfaces;
using ApiForge.Application.GroupUserAssignments.Services;
using ApiForge.Application.Products.Interfaces;
using ApiForge.Application.Products.Services;
using ApiForge.Application.Users.Interfaces;
using ApiForge.Application.Users.Services;
using ApiForge.Application.Products.Validators;
using ApiForge.Application.Groups.Interfaces;
using ApiForge.Application.Groups.Services;
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
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IGroupUserAssignmentService, GroupUserAssignmentService>();

        return services;
    }
}
