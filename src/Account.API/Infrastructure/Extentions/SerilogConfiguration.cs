using Serilog;
namespace Account.API.Infrastructure.Extentions;
public static class SerilogConfiguration
{
    /// <summary>
    /// Static method to create and configure a Serilog ILogger instance
    /// </summary>
    /// <returns>ILogger instance</returns>
    public static Serilog.ILogger CreateSerilogLogger()
    {
        // Set up and return a new Serilog Logger configured as follows:
        return Log.Logger = new LoggerConfiguration()
                               .MinimumLevel.Verbose() // Set the minimum log level to Verbose to capture detailed log data
                               .Enrich.WithProperty("AccountContextLog", "Program") // Add a fixed property "AccountContextLog" with the application name to all log entries
                               .Enrich.FromLogContext() // Enrich log events with additional contextual properties (like thread id, etc.)
                               .WriteTo.File("Logs/.txt", rollingInterval: RollingInterval.Day) // Write log events to a text file, creating a new file every day
                               .CreateLogger(); // Build and return the configured logger
    }
}