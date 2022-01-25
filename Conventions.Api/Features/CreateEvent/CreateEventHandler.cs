using Conventions.Api.Infrastructure;
using Conventions.Domain;
using Conventions.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Conventions.Api.Infrastructure.AuthorizationDefinitions;

namespace Conventions.Api.Features.CreateEvent;

public class CreateEventHandler : IRequestHandler<CreateEventCommand, CreateEventResponse>
{
	private readonly ConventionsDbContext _dbContext;

	public CreateEventHandler(ConventionsDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	public async Task<CreateEventResponse> Handle(CreateEventCommand request, CancellationToken ctx)
	{
		var venue = await _dbContext.Venues.FirstOrDefaultAsync(x => x.Id == request.VenueId, ctx);
		if (venue is null)
		{
			throw CustomHttpException.WrongVenueId(request.VenueId);
		}
		
		var @event = new Event
		{
			Name = request.Name,
			StartDate = request.StartDate,
			EndDate = request.EndDate,
			VenueId = request.VenueId,
			Talks = new List<Talk>(),
		};

		_dbContext.Add(@event);
		await _dbContext.SaveChangesAsync(ctx);

		return new CreateEventResponse(@event.Id);
	}
}

public static class CreateEventHttpHandler
{
	public static WebApplication MapCreateEventHandler(this WebApplication app)
	{
		app.MapPost("/api/events", [Authorize(EventsAdmin)] async (
			[FromBody] CreateEventCommand command, 
			[FromServices] IMediator mediator) =>
		{
			var response = await mediator.Send(command);
			return Results.Created($"/api/events/{response.Id}", response);
		});
		return app;
	}
}