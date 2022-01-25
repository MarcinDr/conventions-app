using MediatR;

namespace Conventions.Api.Features.GetEvents;

public record GetEventsQuery(string? Query) : IRequest<IList<GetEventsResponse>>;