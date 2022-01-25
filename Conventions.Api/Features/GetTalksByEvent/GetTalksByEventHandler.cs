using Conventions.Api.Infrastructure;
using Conventions.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Conventions.Api.Infrastructure.AuthorizationDefinitions;

namespace Conventions.Api.Features.GetTalksByEvent;

public class GetTalksByEventHandler : IRequestHandler<GetTalksByEventQuery, IList<GetTalksByEventResponse>>
{
	private readonly ConventionsDbContext _dbContext;

	public GetTalksByEventHandler(ConventionsDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	public async Task<IList<GetTalksByEventResponse>> Handle(GetTalksByEventQuery request, CancellationToken ctx)
	{
		var eventExists = await _dbContext.Events.AnyAsync(x => x.Id == request.EventId, ctx);
		if (!eventExists)
		{
			throw CustomHttpException.WrongEventId(request.EventId);
		}

		var talks = await _dbContext.Talks
			.Include(x => x.Attendees)
			.Include(x => x.Speaker)
			.Where(x => x.EventId == request.EventId)
			.ToListAsync(ctx);

		return talks
			.Select(t => new GetTalksByEventResponse(t.Id, t.EventId, t.Name, t.StartDate, t.EndDate, 
				new GetTalksByEventAccountResponse(t.Speaker.Id, t.Speaker.Name, t.Speaker.Email),
				t.Attendees.Select(a => new GetTalksByEventAccountResponse(a.Id, a.Name, a.Email)).ToList()))
			.ToList();
	}
}

public static class GetTalksByEventHttpHandler
{
	public static WebApplication MapGetTalksByEventHandler(this WebApplication app)
	{
		app.MapGet("/api/events/{eventId:guid}/talks", [Authorize(TalksRead)] async (
			[FromRoute] Guid eventId, 
			[FromServices] IMediator mediator) =>
		{
			var response = await mediator.Send(new GetTalksByEventQuery(eventId));
			return Results.Ok(response);
		});
		return app;
	}
}