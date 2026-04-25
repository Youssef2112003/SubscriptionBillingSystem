using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SPS.Shared.Abstractions;
using SPS.Shared.Infrastructure.Interceptors;
using SPS.Shared.Infrastructure.Services;
using SPS.Shared.Middleware;
using SPS.Shared.Options;

namespace SPS.Shared.Extensions;

public static class SharedLayerDependencyInjections
{
    public static IServiceCollection AddSharedLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Core Abstractions
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDateTime, DateTimeService>();              
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddSingleton<ICacheService, MemoryCacheService>();


        // Options
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<UploadOptions>(configuration.GetSection(UploadOptions.SectionName));
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));

        // Interceptors
        services.AddScoped<AuditInterceptor>();

        // Caching
        services.AddMemoryCache();

        // Exception Handling (IExceptionHandler)
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        // HttpContext
        services.AddHttpContextAccessor();

        return services;
    }
}