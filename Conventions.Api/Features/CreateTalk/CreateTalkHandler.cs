using Conventions.Api.Infrastructure;
using Conventions.Domain;
using Conventions.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Conventions.Api.Infrastructure.AuthorizationDefinitions;

namespace Conventions.Api.Features.CreateTalk;

public class CreateTalkHandler : IRequestHandler<CreateTalkCommand, CreateTalkResponse>
{
	private readonly ConventionsDbContext _dbContext;

	public CreateTalkHandler(ConventionsDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	public async Task<CreateTalkResponse> Handle(CreateTalkCommand request, CancellationToken ctx)
	{
		var eventExists = await _dbContext.Events.AnyAsync(x => x.Id == request.EventId, ctx);
		if (!eventExists)
		{
			throw CustomHttpException.WrongEventId(request.EventId);
		}

		var speakerExists = await _dbContext.Accounts.AnyAsync(x => x.Id == request.SpeakerId, ctx);
		if (!speakerExists)
		{
			throw CustomHttpException.WrongSpeakerId(request.SpeakerId);
		}

		var talk = new Talk
		{
			Name = request.Name,
			StartDate = request.StartDate,
			EndDate = request.EndDate,
			SpeakerId = request.SpeakerId,
			Attendees = new List<Account>(),
			EventId = request.EventId,
		};

		_dbContext.Add(talk);
		await _dbContext.SaveChangesAsync(ctx);

		return new CreateTalkResponse(talk.Id, talk.EventId);
	}
}

public static class CreateTalkHttpHandler
{
	public static WebApplication MapCreateTalkHandler(this WebApplication app)
	{
		app.MapPost("/api/events/{eventId:guid}/talks", [Authorize(TalksAdmin)] async (
			[FromBody] CreateTalkCommand command, 
			[FromRoute] Guid eventId,
			[FromServices] IMediator mediator) =>
		{
			command.EventId = eventId;
			
			var response = await mediator.Send(command);
			return Results.Created($"/api/events/{response.EventId}/talks/{response.Id}", response);
		});
		return app;
	}
}