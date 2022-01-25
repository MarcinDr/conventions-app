using System.Diagnostics;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Conventions.Api.Infrastructure;

public class ExceptionHandlingMiddleware : IMiddleware
{
	private readonly IWebHostEnvironment _hostEnvironment;

	public ExceptionHandlingMiddleware(IWebHostEnvironment hostEnvironment)
	{
		_hostEnvironment = hostEnvironment;
	}
	
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		try
		{
			await next(context);
		}
		catch (Exception ex)
		{
			await HandleException(context, ex);
		}
	}

	private async Task HandleException(HttpContext context, Exception exception)
	{
		ProblemDetails errorDetails;
		if (exception is IHttpException httpException)
		{
			errorDetails = new ProblemDetails
			{
				Title = exception.Message,
				Status = (int)httpException.StatusCode,
				Instance = context.Request.Path,
				Extensions =
				{
					{ "traceId", Activity.Current?.Id },
					{ "exception", _hostEnvironment.IsProduction() ? null : exception.StackTrace },
				},
			};
		}
		else if (exception is ValidationException validationException)
		{
			var errors = validationException.Errors
				.ToDictionary(e => e.PropertyName, e => new[] { e.ErrorMessage });
			errorDetails = new ValidationProblemDetails(errors)
			{
				Title = exception.Message,
				Status = StatusCodes.Status400BadRequest,
				Instance = context.Request.Path,
				Extensions =
				{
					{ "traceId", Activity.Current?.Id },
					{ "exception", _hostEnvironment.IsProduction() ? null : exception.StackTrace },
				},
			};
		}
		else
		{
			errorDetails = new ProblemDetails
			{
				Title = _hostEnvironment.IsProduction() ? "An internal error occured" : exception.Message,
				Status = StatusCodes.Status500InternalServerError,
				Instance = context.Request.Path,
				Extensions =
				{
					{ "traceId", Activity.Current?.Id },
					{ "exception", _hostEnvironment.IsProduction() ? null : exception.StackTrace },
				},
			};
		}

		context.Response.StatusCode = errorDetails.Status.Value;
		context.Response.ContentType = "application/problem+json";

		await context.Response.WriteAsync(JsonSerializer.Serialize(errorDetails));
	}
}