namespace Conventions.Api.Infrastructure;

public static class ConfigurationLoader
{
	public static IWebHostBuilder UseConfig(this IWebHostBuilder builder)
	{
		builder.ConfigureAppConfiguration(config =>
		{
			config.AddInMemoryCollection(LoadEnvironmentVariables());
		});

		return builder;
	}
	
	private static Dictionary<string, string?> LoadEnvironmentVariables()
    {
        return new()
        {
	        {nameof(AppSettings.ServiceName), Environment.GetEnvironmentVariable("SERVICE_NAME")},
	        
            {$"{nameof(OAuth)}:{nameof(OAuth.Audience)}", Environment.GetEnvironmentVariable("OAUTH_AUDIENCE")},
            {$"{nameof(OAuth)}:{nameof(OAuth.AuthorityUrl)}", Environment.GetEnvironmentVariable("OAUTH_AUTHORITY_URL")},

            {$"{nameof(AppSettings.Database)}:{nameof(AppSettings.Database.Host)}", Environment.GetEnvironmentVariable("PGHOST")},
            {$"{nameof(AppSettings.Database)}:{nameof(AppSettings.Database.Name)}", Environment.GetEnvironmentVariable("PGDATABASE")},
            {$"{nameof(AppSettings.Database)}:{nameof(AppSettings.Database.Username)}", Environment.GetEnvironmentVariable("PGUSER")},
            {$"{nameof(AppSettings.Database)}:{nameof(AppSettings.Database.Password)}", Environment.GetEnvironmentVariable("PGPASSWORD")},
            {$"{nameof(AppSettings.Database)}:{nameof(AppSettings.Database.Port)}", Environment.GetEnvironmentVariable("PGPORT")},
        };
    }
}