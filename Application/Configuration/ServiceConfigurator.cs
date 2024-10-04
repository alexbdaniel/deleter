using System.Runtime.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Configuration;

[SupportedOSPlatform("Windows")]
public static class ServiceConfigurator
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.ConfigureOptions(configuration);
        
        return services;
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