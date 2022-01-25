namespace Conventions.Api.Features.GetEvents;

public record GetEventsResponse(Guid Id, string Name, DateTime StartDate, DateTime EndDate, Guid VenueId);