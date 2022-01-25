using System.Text.Json;
using System.Text.Json.Serialization;
using Auth0.AspNetCore.Authentication;
using Conventions.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Yarp.ReverseProxy.Forwarder;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseConfig();

var appSettings = builder.Configuration.Get<AppSettings>();

var services = builder.Services;
services.AddHttpForwarder();
services
	.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	});

services.AddConfiguredAuthentication(appSettings)
	.AddConfiguredSpaStaticFiles(appSettings);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseHsts();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();

app.UseAuthentication();

app.Use(async (context, next) =>
{
	if (context.User.Identity is null || !context.User.Identity.IsAuthenticated)
	{
		var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
			.WithRedirectUri("/")
			.Build();

		await context.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
		// await context.ChallengeAsync();
		return;
	}

	await next(context);
});

app.UseAuthorization();

app.UseSpaStaticFilesConfigured();
app.UseRouting();

app.UseConfiguredProxy(appSettings.ForwardUrl, app.Services.GetRequiredService<IHttpForwarder>());
app.UseSpaConfigured();
// app.MapFallbackToFile("index.html");

await app.RunAsync();