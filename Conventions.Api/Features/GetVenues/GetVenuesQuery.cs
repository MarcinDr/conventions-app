using MediatR;

namespace Conventions.Api.Features.GetVenues;

public record GetVenuesQuery(string? Query) : IRequest<IList<GetVenuesResponse>>;