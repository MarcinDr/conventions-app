namespace Conventions.Api.Infrastructure;
#pragma warning disable 8618

public class AppSettings
{
	public string ServiceName { get; set; }
	public OAuth OAuth { get; set; }
	public Database Database { get; set; }
}

public class OAuth
{
	public string Audience { get; set; }
	public string AuthorityUrl { get; set; }
}

public class Database
{
	public string ConnectionString
		=> $"Host={Host};Port={Port};Database={Name};Username={Username};Password={Password};SSL Mode=Prefer;Trust Server Certificate=true;";
	public string Host { get; set; }
	public string Name { get; set; }
	public int Port { get; set; }
	public string Username { get; set; }
	public string Password { get; set; }
}