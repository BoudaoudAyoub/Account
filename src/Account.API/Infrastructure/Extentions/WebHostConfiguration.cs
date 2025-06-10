using Autofac;
using Serilog;
using System.Net;
using Account.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Account.API.Infrastructure.Modules;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Account.API.Infrastructure.Modules.AutoMapper;
using Account.API.Infrastructure.Seed;
namespace Account.API.Infrastructure.Extentions;

public static class WebHostConfiguration
{
    public static IHost BuildWebHost(IConfiguration configuration, string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                   .ConfigureWebHostDefaults(webBuilder =>
                   {
                       webBuilder.CaptureStartupErrors(false)
                                 .ConfigureKestrel(ConfigureKestrelWebServer())
                                 .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                                 .ConfigureServices(ConfigureServices(configuration))
                                 .Configure(ConfigureApplication())
                                 .UseContentRoot(Directory.GetCurrentDirectory())                       
                                 .UseUrls($"https://localhost:403");
                   })
                   .ConfigureContainer(ConfigureContainers())
                   .UseSerilog()
                   .Build();
    }

    private static Action<KestrelServerOptions> ConfigureKestrelWebServer()
    {
        return options =>
        {
            options.Listen(IPAddress.Any, 403, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });
        };
    }

    private static Action<IServiceCollection> ConfigureServices(IConfiguration configuration)
    {
        return services =>
        {
            services.AddCustomDbContext(configuration);
            services.AddCustomSwagger(configuration);
            services.AddCustomConfiguration(configuration);
            services.AddAutofac();
            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<AccountDbContext>()
                    .AddDefaultTokenProviders();

            services.Seeder();
        };
    }

    private static Action<WebHostBuilderContext, IApplicationBuilder> ConfigureApplication()
    {
        return (whbc, app) =>
        {
            app.AddCustomConfigure(whbc.HostingEnvironment);
        };
    }

    private static Action<ContainerBuilder> ConfigureContainers()
    {
        return container =>
        {
            container.RegisterModule<MediatorModule>();
            container.RegisterModule<AutoMapperLoad>();
            container.RegisterModule<ApplicationModule>();
            //TODO: register more modules to the container builder
        };
    }
}