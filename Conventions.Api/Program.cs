using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;
using Conventions.Api.Features;
using Conventions.Api.Infrastructure;
using Conventions.Api.Services;
using Conventions.Domain;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Refit;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseConfig();

var appSettings = builder.Configuration.Get<AppSettings>();

var services = builder.Services;
services
	.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
	});

services.AddMvc()
	.AddFluentValidation(fv =>
	{
		fv.DisableDataAnnotationsValidation = true;
		fv.RegisterValidatorsFromAssemblyContaining<Program>();
	});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddRefitClient<IBreweriesApi>()
	.ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.openbrewerydb.org"));
services.AddTransient<AppInitializationService>()
	.AddScoped<ExceptionHandlingMiddleware>();
services.AddConfiguredDbContext(appSettings.Database.ConnectionString)
	.AddMediatR(typeof(Program));

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
	{
		options.Authority = appSettings.OAuth.AuthorityUrl;
		options.Audience = appSettings.OAuth.Audience;
	});

services.AddAuthorizationPolicies();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHttpHandlers();
app.MapControllers();

var appInitializationService = app.Services.GetRequiredService<AppInitializationService>();
await appInitializationService.Initialize();

await app.RunAsync();