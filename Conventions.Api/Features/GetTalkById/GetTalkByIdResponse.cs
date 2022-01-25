namespace Conventions.Api.Features.GetTalkById;

public record GetTalkByIdResponse(Guid Id,
	Guid EventId,
	string Name, 
	DateTime StartDate, 
	DateTime EndDate, 
	GetTalksByIdAccountResponse Speaker,
	IList<GetTalksByIdAccountResponse> Attendees);
	
	
public record GetTalksByIdAccountResponse(string Id, string? Name, string Email);