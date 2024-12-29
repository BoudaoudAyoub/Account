﻿using System.Reflection;
using Account.API.Infrastructure.Filters;
using Account.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
namespace Account.API.Infrastructure.Extentions;
public static class CustomExtensionMethods
{
    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDbContext<AccountDbContext>(options =>
        {
            options.UseSqlServer(configuration["CustomerManConnectionString"], sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.MigrationsHistoryTable("AccountMigrationsHistory");
                sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(60), errorNumbersToAdd: null);
            });
        },
        //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
        ServiceLifetime.Scoped);

        return services;
    }

    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Account - API",
                Version = "v1",
                Description = "Account API"
            });
        });

        return services;
    }

    public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions();
        services.AddSwaggerGen();
        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(CustomExceptionFilter));
            options.Filters.Add(typeof(CustomActionFilter));
        });
        services.AddEndpointsApiExplorer();
        services.AddAutoMapper(typeof(Program).GetTypeInfo().Assembly);
        return services;
    }

    public static IApplicationBuilder AddCustomConfigure(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseSwagger();

        app.UseHttpsRedirection();

        app.UseAuth();

        app.UseRouting();

        app.UseEndpoints(endpoint =>
        {
            endpoint.MapDefaultControllerRoute();
            endpoint.MapControllers();
        });

        return app;
    }

    private static IApplicationBuilder UseSwagger(this IApplicationBuilder app)
    {
        return SwaggerBuilderExtensions.UseSwagger(app).UseSwaggerUI();
    }

    private static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        return app.UseAuthentication()
                  .UseAuthorization();
    }
}