using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Conventions.Domain;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddConfiguredDbContext(this IServiceCollection services, string connectionString)
	{
		return services.AddDbContextPool<ConventionsDbContext>(options =>
		{
			options.UseNpgsql(connectionString);
			options.UseSnakeCaseNamingConvention();
		});
	}
}