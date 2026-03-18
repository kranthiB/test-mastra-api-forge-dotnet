using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiForge.Api.Extensions;

/// <summary>
/// Dynamically creates a <see cref="OpenApiInfo"/> for each discovered API version
/// so Swagger UI shows a separate doc per version.
/// </summary>
public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfo(description));
        }
    }

    private static OpenApiInfo CreateInfo(ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title       = "ApiForge – Production-Grade API Template",
            Version     = description.GroupName,
            Description = "Clean Architecture CRUD template for .NET 10. " +
                          "Replace the in-memory repositories with your real data store.",
            Contact = new OpenApiContact
            {
                Name  = "Platform Team",
                Email = "platform@example.com",
            },
            License = new OpenApiLicense
            {
                Name = "MIT",
                Url  = new Uri("https://opensource.org/licenses/MIT"),
            },
        };

        if (description.IsDeprecated)
            info.Description += " **This API version is deprecated.**";

        return info;
    }
}
