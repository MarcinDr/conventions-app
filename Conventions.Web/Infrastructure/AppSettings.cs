namespace Conventions.Web.Infrastructure;

public class AppSettings
{
	public string ForwardUrl { get; set; }
	public OAuth OAuth { get; set; }
}

public class OAuth
{
	public string ClientId { get; set; }
	public string ClientSecret { get; set; }
	public string AllowedScopes { get; set; }
	public string AuthorityUrl { get; set; }

	public IEnumerable<string> Scopes => AllowedScopes.Split(" ", StringSplitOptions.RemoveEmptyEntries);
}