using Conventions.Web.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class SpaServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguredSpaStaticFiles(this IServiceCollection services, AppSettings appSettings)
    {
        // In production, the React files will be served from this directory
        services.AddSpaStaticFiles(configuration =>
        {
            configuration.RootPath = "build"; 
        });

        return services;
    }
}