using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SPS.Shared.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services,
        string apiTitle,
        string apiVersion = "v1",
        string? xmlCommentsFileName = null)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(apiVersion, new OpenApiInfo
            {
                Title = apiTitle,
                Version = apiVersion,
                Description = $"{apiTitle} API Documentation"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });

            if (!string.IsNullOrEmpty(xmlCommentsFileName))
            {
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFileName);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            }

            c.CustomSchemaIds(type => type.FullName?.Replace("+", ".")); 
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerWithUI(this IApplicationBuilder app,
        string apiName,
        string routePrefix = "swagger")
    {
        app.UseSwagger(c => c.RouteTemplate = "swagger/{documentName}/swagger.json");
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", apiName);
            c.RoutePrefix = routePrefix;
            c.DocExpansion(DocExpansion.None);
            c.DisplayRequestDuration();
            c.DefaultModelsExpandDepth(-1); 
            c.EnableDeepLinking();
            c.EnableValidator();
        });

        return app;
    }
}