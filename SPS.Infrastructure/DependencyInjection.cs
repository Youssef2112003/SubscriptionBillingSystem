using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SPS.Application.Common;
using SPS.Domain.Repositories;
using SPS.Infrastructure.Jobs;
using SPS.Infrastructure.Options;
using SPS.Infrastructure.Outbox;
using SPS.Infrastructure.Persistence;
using SPS.Infrastructure.Repositories;
using SPS.Infrastructure.Services;

namespace SPS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));  // or UseInMemoryDatabase("SPSDb"))

            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

            // Repositories
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            // Application abstractions implementation
            services.AddScoped<IDateTime, DateTimeService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddScoped<IFileStorageService, FileStorageService>();

            // Outbox
            services.AddHostedService<OutboxProcessor>();


            services.AddHttpContextAccessor();


            // Billing background job
            services.AddHostedService<BillingCycleService>();

            // Memory cache
            services.AddMemoryCache();

            services.Configure<SmtpOptions>(options =>
    configuration.GetSection(SmtpOptions.SectionName).Bind(options));
            services.Configure<FileStorageOptions>(options =>
                configuration.GetSection(FileStorageOptions.SectionName).Bind(options));

            return services;
        }
    }
}
