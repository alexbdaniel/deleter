using Application.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application;

internal static class Program
{
    private static async Task Main()
    {
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        
        using IHost host = new HostBuilder().Build();

        await host.StartAsync();
        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

        var configuration = new ConfigurationManager()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.development.json", true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();

        services.ConfigureServices(configuration);

        await using var provider = services.BuildServiceProvider();

        var deleter = provider.GetRequiredService<Deleter>();
        var failures = deleter.StartDeleting();
        WriteResultToConsole(failures);
        
        lifetime.StopApplication();
        
        Console.WriteLine("Shutting down");
        await host.WaitForShutdownAsync();
    }

    private static void WriteResultToConsole(List<Failure> failures)
    {
        if (failures.Count == 0)
        {
            Console.WriteLine("Completed successfully.");
            return;
        }
        
        string title = $"The following failure{(failures.Count > 1 ? "s" : "")} occured:";
        Console.WriteLine(title);
        
        foreach (var failure in failures)
        {
            Console.WriteLine("");
            Console.WriteLine($" - {failure.Message}");
            Console.WriteLine($"    Reason  = \"{failure.Reason}\"");
            Console.WriteLine($"    Path    = \"{failure.Path}\"");
            Console.WriteLine("");
        }
        
        
    }
    
    private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e) {
        Console.WriteLine(e.ExceptionObject.ToString());
        Console.WriteLine("Press Enter to continue");
        Console.ReadLine();
        Environment.Exit(1);
    }
}