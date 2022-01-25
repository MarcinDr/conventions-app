using System.IdentityModel.Tokens.Jwt;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Conventions.Web.Infrastructure;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddConfiguredAuthentication(this IServiceCollection services,
        AppSettings appSettings)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services.AddAccessTokenManagement();
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddAuth0WebAppAuthentication(options =>
            {
                options.Domain = appSettings.OAuth.AuthorityUrl;
                options.ClientId = appSettings.OAuth.ClientId;
                options.ClientSecret = appSettings.OAuth.ClientSecret;
                options.Scope = appSettings.OAuth.AllowedScopes;

                options.ResponseType = "code";
                options.OpenIdConnectEvents ??= new OpenIdConnectEvents();
                options.OpenIdConnectEvents.OnTokenResponseReceived = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

                    logger.LogInformation(context.TokenEndpointResponse.AccessToken);

                    return Task.CompletedTask;
                };
            })
            .WithAccessToken(o =>
            {
                o.Audience = "venues.api";
                o.Scope = "venues.admin venues.read events.admin events.read talks.admin talks.read attendance.admin";
                o.UseRefreshTokens = true;
            });

        return services;
    }
}