using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Conventions.Api.Infrastructure;

public static class AuthorizationDefinitions
{
	private const string PermissionsClaim = "permissions";
	
	public const string EventsAdmin = nameof(EventsAdmin);
	public const string TalksAdmin = nameof(TalksAdmin);
	public const string VenuesAdmin = nameof(VenuesAdmin);
	public const string AttendanceAdmin = nameof(AttendanceAdmin);
	public const string EventsRead = nameof(EventsRead);
	public const string TalksRead = nameof(TalksRead);
	public const string VenuesRead = nameof(VenuesRead);

	public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
	{
		services.AddAuthorization(options =>
		{
			options.FallbackPolicy = options.DefaultPolicy;
			
			options.AddPolicy(EventsAdmin, EventsAdminPolicy());
			options.AddPolicy(TalksAdmin, TalksAdminPolicy());
			options.AddPolicy(VenuesAdmin, VenuesAdminPolicy());
			options.AddPolicy(AttendanceAdmin, AttendanceAdminPolicy());
			options.AddPolicy(EventsRead, EventsReadPolicy());
			options.AddPolicy(TalksRead, TalksReadPolicy());
			options.AddPolicy(VenuesRead, VenuesReadPolicy());
		});
		return services;
	}

	private static AuthorizationPolicy EventsAdminPolicy() =>
		CreateDefaultBuilder()
			.RequireClaim(PermissionsClaim, "events.admin")
			.Build();
	
	private static AuthorizationPolicy TalksAdminPolicy() =>
		CreateDefaultBuilder()
			.RequireClaim(PermissionsClaim, "talks.admin")
			.Build();
	
	private static AuthorizationPolicy VenuesAdminPolicy() =>
		CreateDefaultBuilder()
			.RequireClaim(PermissionsClaim, "venues.admin")
			.Build();
	
	private static AuthorizationPolicy AttendanceAdminPolicy() =>
		CreateDefaultBuilder()
			.RequireClaim(PermissionsClaim, "attendance.admin")
			.Build();
	
	private static AuthorizationPolicy EventsReadPolicy() =>
		CreateDefaultBuilder()
			.RequireClaim(PermissionsClaim, "events.read")
			.Build();
	
	private static AuthorizationPolicy TalksReadPolicy() =>
		CreateDefaultBuilder()
			.RequireClaim(PermissionsClaim, "talks.read")
			.Build();
	
	private static AuthorizationPolicy VenuesReadPolicy() =>
		CreateDefaultBuilder()
			.RequireClaim(PermissionsClaim, "venues.read")
			.Build();
	
	private static AuthorizationPolicyBuilder CreateDefaultBuilder()
	{
		var policyBuilder = new AuthorizationPolicyBuilder();
		policyBuilder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
		policyBuilder.RequireAuthenticatedUser();

		return policyBuilder;
	}
}