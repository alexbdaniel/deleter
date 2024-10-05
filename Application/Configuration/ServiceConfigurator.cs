using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Syslog;

namespace Application.Configuration;

[SupportedOSPlatform("Windows")]
public static class ServiceConfigurator
{
    private const string applicationName = "file-cleaner"; 
    
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.ConfigureLogging(configuration);
        services.ConfigureOptions(configuration);
        
        return services;
    }

    private static IServiceCollection ConfigureLogging(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var logger = new LoggerConfiguration()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteToLocalLogSystem(services)
            .CreateLogger();
        
       
        services.AddLogging(configure => { configure.AddSerilog(logger); });

        logger.Information("Logging configured.");
        
        
        return services;
    }

    private static LoggerConfiguration WriteToLocalLogSystem(this LoggerConfiguration logger, IServiceCollection services)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            logger.WriteTo.LocalSyslog(applicationName, Facility.Syslog);
            return logger;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                logger.WriteTo.EventLog(applicationName, manageEventSource: true);
            }
            catch (SecurityException ex)
            {
                string parentDirectoryName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string directoryName = Path.Combine(parentDirectoryName, "FileCleaner", "Log");
                Directory.CreateDirectory(directoryName);
                
                Console.WriteLine($"Cannot log to EventLog. Will instead attempt to log to \"{directoryName}\".");
            }

            return logger;
        }
        
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Console.WriteLine("Unsupported OS platform for logging.");
        
        return logger;
    }
    
    private static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddOptions<DeleteOptions[]>().Bind(configuration.GetSection(DeleteOptions.Key))
            .ValidateDataAnnotations()
            .Validate(options => new DeleteOptionsValidator().Validate(options))
            .ValidateOnStart();
        
        return services;
    }
}