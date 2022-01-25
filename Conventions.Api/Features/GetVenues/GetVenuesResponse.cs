namespace Conventions.Api.Features.GetVenues;

public record GetVenuesResponse(Guid Id,
	string Name,
	string Street,
	string City,
	string Country,
	string PostalCode);