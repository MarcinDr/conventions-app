using Conventions.Domain;
using Conventions.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Conventions.Api.Infrastructure.AuthorizationDefinitions;

namespace Conventions.Api.Features.GetEvents;

public class GetEventsHandler : IRequestHandler<GetEventsQuery, IList<GetEventsResponse>>
{
	private readonly ConventionsDbContext _dbContext;

	public GetEventsHandler(ConventionsDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	public async Task<IList<GetEventsResponse>> Handle(GetEventsQuery request, CancellationToken ctx)
	{
		List<Event> events;
		if (!string.IsNullOrWhiteSpace(request.Query))
		{
			events = await _dbContext.Events
				.Where(e => EF.Functions.ILike(e.Name, $"%{request.Query}%"))
				.ToListAsync(ctx);
		}
		else
		{
			events = await _dbContext.Events.ToListAsync(ctx);
		}
		
		return events
			.Select(e => new GetEventsResponse(e.Id, e.Name, e.StartDate, e.EndDate, e.VenueId))
			.ToList();
	}
}

public static class GetEventsHandlerHttpHandler
{
	public static WebApplication MapGetEventsHandler(this WebApplication app)
	{
		app.MapGet("/api/events", [Authorize(EventsRead)] async (
			HttpContext context,
			[FromQuery] string? query, 
			[FromServices] IMediator mediator) =>
		{
			var response = await mediator.Send(new GetEventsQuery(query));
			return Results.Ok(response);
		});
		return app;
	}
}