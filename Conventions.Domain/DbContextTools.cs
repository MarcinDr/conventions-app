namespace Conventions.Domain;

internal static class DbContextTools
{
	private const string UserIdParam = "userId";
	private const string PasswordParam = "password";
	private const string DatabaseParam = "db";

	public static string GetDbContextFactoryLocalHostConnectionString(string[] args)
	{
		var userId = GetFromArguments(args, UserIdParam, "conventions");
		var password = GetFromArguments(args, PasswordParam, "conventions");
		var database = GetFromArguments(args, DatabaseParam, "conventions");

		return $"User ID={userId};Password={password};Host=localhost;Port=5432;Database={database};Include Error Detail=true;";
	}

	private static string GetFromArguments(string[] args, string key, string defaultValue)
	{
		var value = args.FirstOrDefault(x => x.Contains(key));
		if (string.IsNullOrWhiteSpace(value))
		{
			return defaultValue;
		}

		var split = value.Split(":", StringSplitOptions.RemoveEmptyEntries);
		return split.Last();
	}
}