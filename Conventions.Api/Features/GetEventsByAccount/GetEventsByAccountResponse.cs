namespace Conventions.Api.Features.GetEventsByAccount;

public record GetEventsByAccountResponse(IList<GetEventsByAccountResponseBody> AsSpeaker,
	IList<GetEventsByAccountResponseBody> AsAttendee);

public record GetEventsByAccountResponseBody(Guid Id, string Name, DateTime StartDate, DateTime EndDate, Guid EventId);