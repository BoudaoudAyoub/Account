using Serilog;
using System.Reflection;
using Account.API.Infrastructure.Extentions;

SerilogConfiguration.CreateSerilogLogger();
var appName = Assembly.GetExecutingAssembly().GetName().Name;

try
{
    Log.Information("Configuring web host ({AccountContext})...", appName);
    var configuration = CustomConfigurationBuilder.GetConfiguration();
    var host = WebHostConfiguration.BuildWebHost(configuration, args);

    Log.Information("Starting web host ({AccountContext})...", appName);
    await host.RunAsync();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly ({AccountContext})!", appName);
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}