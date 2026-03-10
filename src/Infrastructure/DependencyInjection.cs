using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Infrastructure.Data;
using Shop_Cam_BE.Infrastructure.Services;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });

        services.AddHttpClient<IKeycloakService, KeycloakService>();

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConnection + ",abortConnect=false"));
            services.AddScoped<IRedisService, RedisService>();
        }
        else
        {
            services.AddScoped<IRedisService, InMemoryRedisStub>();
        }

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IActivityLogService, NoOpActivityLogService>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
