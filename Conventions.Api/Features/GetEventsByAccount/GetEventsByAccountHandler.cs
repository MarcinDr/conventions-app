using Conventions.Api.Infrastructure;
using Conventions.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Conventions.Api.Infrastructure.AuthorizationDefinitions;

namespace Conventions.Api.Features.GetEventsByAccount;

public class GetEventsByAccountHandler : IRequestHandler<GetEventByAccountQuery, GetEventsByAccountResponse>
{
	private readonly ConventionsDbContext _dbContext;

	public GetEventsByAccountHandler(ConventionsDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	public async Task<GetEventsByAccountResponse> Handle(GetEventByAccountQuery request, CancellationToken ctx)
	{
		var account = await _dbContext.Accounts
			.Include(x => x.TalksAsAttendee)
			.Include(x => x.TalksAsSpeaker)
			.FirstOrDefaultAsync(x => x.Id == request.AccountId, ctx);

		if (account is null)
		{
			throw CustomHttpException.SorryAccountNotFound();
		}

		var asSpeaker = account.TalksAsSpeaker.Select(t => 
			new GetEventsByAccountResponseBody(t.Id, t.Name, t.StartDate, t.EndDate, t.EventId)).ToList();
		
		var asAttendee = account.TalksAsAttendee.Select(t => 
			new GetEventsByAccountResponseBody(t.Id, t.Name, t.StartDate, t.EndDate, t.EventId)).ToList();
		
		return new GetEventsByAccountResponse(asSpeaker, asAttendee);
	}
}

public static class GetEventsByAccountHttpHandler
{
	public static WebApplication MapGetEventsByAccountHandler(this WebApplication app)
	{
		app.MapGet("/api/events/mine", [Authorize(EventsRead)] async (
			HttpContext context,
			[FromServices] IMediator mediator) =>
		{
			var response = await mediator.Send(new GetEventByAccountQuery(context.GetAccountId()));
			return Results.Ok(response);
		});
		return app;
	}
}