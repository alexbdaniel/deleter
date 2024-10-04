using System.Runtime.Versioning;
using Application.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application;

[SupportedOSPlatform("Windows")]
internal static class Program
{
    private static async Task Main()
    {
        using IHost host = new HostBuilder().Build();

        await host.StartAsync();
        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

        var configuration = new ConfigurationManager()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.development.json", true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();

        services.ConfigureServices(configuration);

        await using var provider = services.BuildServiceProvider();

        var deleter = provider.GetRequiredService<Deleter>();
        deleter.StartDeleting();
        
        lifetime.StopApplication();
        
        Console.WriteLine("Shutting down");
        await host.WaitForShutdownAsync();
        

        
        
        
        
        Console.WriteLine("Hello, World!");
    }
}