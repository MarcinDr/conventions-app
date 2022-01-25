using System.Security.Claims;

namespace Conventions.Api.Infrastructure;

public static class HttpContextExtensions
{
	public static string GetAccountId(this HttpContext context)
	{
		if (context.User.Identity is null)
		{
			// somehow request is not authenticated
			throw CustomHttpException.Unauthorized();
		}

		var sub = context.User.FindFirstValue("sub");
		if (string.IsNullOrWhiteSpace(sub))
		{
			throw CustomHttpException.Unauthorized("Account id is missing");
		}
		return sub;
	}
}