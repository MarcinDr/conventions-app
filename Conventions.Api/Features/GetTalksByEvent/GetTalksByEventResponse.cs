namespace Conventions.Api.Features.GetTalksByEvent;

public record GetTalksByEventResponse(Guid Id,
	Guid EventId,
	string Name, 
	DateTime StartDate, 
	DateTime EndDate, 
	GetTalksByEventAccountResponse Speaker,
	IList<GetTalksByEventAccountResponse> Attendees);

public record GetTalksByEventAccountResponse(string Id, string? Name, string Email);