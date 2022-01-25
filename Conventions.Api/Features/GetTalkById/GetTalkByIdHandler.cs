using Conventions.Api.Infrastructure;
using Conventions.Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Conventions.Api.Infrastructure.AuthorizationDefinitions;

namespace Conventions.Api.Features.GetTalkById;

public class GetTalkByIdHandler : IRequestHandler<GetTalkByIdQuery, GetTalkByIdResponse>
{
	private readonly ConventionsDbContext _dbContext;

	public GetTalkByIdHandler(ConventionsDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	public async Task<GetTalkByIdResponse> Handle(GetTalkByIdQuery request, CancellationToken ctx)
	{
		var eventExists = await _dbContext.Events.AnyAsync(x => x.Id == request.EventId, ctx);
		if (!eventExists)
		{
			throw CustomHttpException.WrongEventId(request.EventId);
		}

		var talk = await _dbContext.Talks
			.Include(x => x.Speaker)
			.Include(x => x.Attendees)
			.FirstOrDefaultAsync(x => x.Id == request.Id, ctx);
		if (talk is null)
		{
			throw CustomHttpException.TalkNotFound(request.Id, request.EventId);
		}

		return new GetTalkByIdResponse(talk.Id, talk.EventId, talk.Name, talk.StartDate, talk.EndDate,
			new GetTalksByIdAccountResponse(talk.Speaker.Id, talk.Speaker.Name, talk.Speaker.Email),
			talk.Attendees.Select(a => new GetTalksByIdAccountResponse(a.Id, a.Name, a.Email)).ToList());
	}
}

public static class GetTalksByIdHttpHandler
{
	public static WebApplication MapGetTalksByIdHandler(this WebApplication app)
	{
		app.MapGet("/api/events/{eventId:guid}/talks/{talkId:guid}", [Authorize(TalksRead)] async (
			[FromRoute] Guid eventId,
			[FromRoute] Guid talkId,
			[FromServices] IMediator mediator) =>
		{
			var query = new GetTalkByIdQuery(talkId, eventId);
			var queryValidator = new GetTalkByIdQueryValidator();
			var validationResult = queryValidator.Validate(query);
			
			if (!validationResult.IsValid)
			{
				throw new ValidationException(validationResult.Errors);
			}
			
			var response = await mediator.Send(query);
			return Results.Ok(response);
		});
		return app;
	}
}