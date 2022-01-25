using System.Net;

namespace Conventions.Api.Infrastructure;

public interface IHttpException
{
	HttpStatusCode StatusCode { get; }
	string? Message { get; }
}

public class CustomHttpException : Exception, IHttpException
{
	public HttpStatusCode StatusCode { get; }

	private CustomHttpException(HttpStatusCode statusCode, string? message) : base(message)
	{
		StatusCode = statusCode;
	}

	public static CustomHttpException SorryAccountNotFound() 
		=> new(HttpStatusCode.Conflict, "Sorry...your account has not been hardcoded, you can't use our platform");

	public static CustomHttpException Unauthorized(string? message = null)
		=> new(HttpStatusCode.Unauthorized, message);

	public static CustomHttpException WrongVenueId(Guid venueId)
		=> BadRequest($"Venue with id {venueId} not found.");
	
	public static CustomHttpException WrongEventId(Guid eventId)
		=> BadRequest($"Event with id {eventId} not found.");
	
	public static CustomHttpException WrongSpeakerId(string speakerId)
		=> BadRequest($"Speaker with id {speakerId} not found.");
	
	public static CustomHttpException AttendeeNotFound(string accountId)
		=> BadRequest($"Account with id {accountId} not found.");

	public static CustomHttpException TalkNotFound(Guid talkId, Guid eventId)
		=> NotFound($"Talk with id {talkId} for event {eventId} was not found.");

	private static CustomHttpException BadRequest(string message) => new(HttpStatusCode.BadRequest, message);
	private static CustomHttpException NotFound(string message) => new(HttpStatusCode.NotFound, message);
}