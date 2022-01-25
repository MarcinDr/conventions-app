using Microsoft.Net.Http.Headers;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class SpaAppBuilderExtensions
{
    public static IApplicationBuilder UseSpaStaticFilesConfigured(this IApplicationBuilder app)
    {
        app.UseStaticFiles();
        app.UseSpaStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                if (ctx.Context.Request.Path.StartsWithSegments("/static"))
                {
                    // Cache all static resources for 1 year (versioned filenames)
                    var headers = ctx.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromDays(365),
                    };
                }
                else
                {
                    // Do not cache explicit `/index.html` or any other files.  See also: `DefaultPageStaticFileOptions` below for implicit "/index.html"
                    var headers = ctx.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromDays(0),
                    };
                }
            },
        });
        return app;
    }

    public static IApplicationBuilder UseSpaConfigured(this IApplicationBuilder app)
    {
        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "ClientApp/public";
            spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // Do not cache implicit `/index.html`.  See also: `UseSpaStaticFiles` above
                    var headers = ctx.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromDays(0),
                    };
                },
            };

            spa.UseProxyToSpaDevelopmentServer("https://localhost:44498");
        });

        return app;
    }
}