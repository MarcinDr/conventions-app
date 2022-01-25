namespace Conventions.Web.Infrastructure;

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
	        {nameof(AppSettings.ForwardUrl), Environment.GetEnvironmentVariable("FORWARD_URL")},
	        
	        {$"{nameof(OAuth)}:{nameof(OAuth.ClientId)}", Environment.GetEnvironmentVariable("OAUTH_CLIENT_ID")},
            {$"{nameof(OAuth)}:{nameof(OAuth.ClientSecret)}", Environment.GetEnvironmentVariable("OAUTH_CLIENT_SECRET")},
            {$"{nameof(OAuth)}:{nameof(OAuth.AllowedScopes)}", Environment.GetEnvironmentVariable("OAUTH_ALLOWED_SCOPES")},
            {$"{nameof(OAuth)}:{nameof(OAuth.AuthorityUrl)}", Environment.GetEnvironmentVariable("OAUTH_AUTHORITY_URL")},
        };
    }
}