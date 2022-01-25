using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;
using Yarp.ReverseProxy.Forwarder;

namespace Conventions.Web.Infrastructure;

public static class AppBuilderExtensions
{
	public static IApplicationBuilder UseConfiguredProxy(this IApplicationBuilder app, 
		string forwardingUrl, 
		IHttpForwarder httpForwarder)
	{
		var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
		{
			UseProxy = false,
			AllowAutoRedirect = false,
			AutomaticDecompression = DecompressionMethods.None,
			UseCookies = false
		});
		var transformer = new CustomTransformer(); // or HttpTransformer.Default;
		var requestConfig = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(100) };

		app.UseEndpoints(endpoints =>
		{
			endpoints.Map("/api/{**catch-all}", async context =>
			{
				var error = await httpForwarder.SendAsync(context, forwardingUrl,
					httpClient, requestConfig, transformer);
				// Check if the operation was successful
				if (error != ForwarderError.None)
				{
					var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
					var errorFeature = context.GetForwarderErrorFeature();
					var exception = errorFeature?.Exception;
					if (exception is not null)
					{
						logger.LogError(exception, "Error while forwarding to downstream service");
					}
				}
			});
		});
		return app;
	}
}

public class CustomTransformer : HttpTransformer
{
	public override async ValueTask TransformRequestAsync(HttpContext httpContext, HttpRequestMessage proxyRequest, string destinationPrefix)
	{
		var accessToken = await httpContext.GetTokenAsync("access_token");
		httpContext.Request.Headers.Add(HeaderNames.Authorization, $"Bearer {accessToken}");
		
		// Copy headers normally and then remove the original host.
		// Use the destination host from proxyRequest.RequestUri instead.
		await base.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix);
		proxyRequest.Headers.Host = null;
	}
	
	public override ValueTask<bool> TransformResponseAsync(HttpContext httpContext, HttpResponseMessage? proxyResponse)
	{
		if (proxyResponse is not null)
		{
			AddCacheControlHeader(proxyResponse);
			AddXssProtectionHeader(proxyResponse);
			AddContentTypeOptionsHeader(proxyResponse);
			AddFrameOptionsHeader(proxyResponse);
			AddContentSecurityPolicyHeader(proxyResponse);

			RemoveHeader(proxyResponse, "Server");
		}

		return base.TransformResponseAsync(httpContext, proxyResponse);
	}
	
	private static void AddCacheControlHeader(HttpResponseMessage response)
		=> AddHeaderIfNotExists(response, "Cache-Control", "no-store");

	private static void AddXssProtectionHeader(HttpResponseMessage response)
		=> AddHeaderIfNotExists(response, "X-XSS-Protection", "0");

	private static void AddContentTypeOptionsHeader(HttpResponseMessage response)
		=> AddHeaderIfNotExists(response, "X-Content-Type-Options", "nosniff");

	private static void AddFrameOptionsHeader(HttpResponseMessage response)
		=> AddHeaderIfNotExists(response, "X-Frame-Options", "DENY");

	private static void AddContentSecurityPolicyHeader(HttpResponseMessage response)
		=> AddHeaderIfNotExists(response, "Content-Security-Policy", "default-src 'none'; frame-ancestors 'none'; sandbox");

	private static void AddHeaderIfNotExists(HttpResponseMessage response, string key, string value)
	{
		if (!response.Headers.Contains(key))
		{
			response.Headers.Add(key, value);
		}
	}

	private static void RemoveHeader(HttpResponseMessage response, string key)
	{
		response.Headers.Remove(key);
	}
}