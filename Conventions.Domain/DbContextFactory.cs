using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Conventions.Domain;

public class ConventionsDbContextFactory : IDesignTimeDbContextFactory<ConventionsDbContext>
{
	public ConventionsDbContext CreateDbContext(string[] args)
	{
		var options = new DbContextOptionsBuilder<ConventionsDbContext>();
		var connectionString = DbContextTools.GetDbContextFactoryLocalHostConnectionString(args);

		options.UseNpgsql(connectionString,
				assembly => assembly.MigrationsAssembly(typeof(ConventionsDbContextFactory).GetTypeInfo().Assembly.GetName().Name))
			.UseSnakeCaseNamingConvention();

		return new ConventionsDbContext(options.Options);
	}
}