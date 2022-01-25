using Conventions.Api.Infrastructure;
using Conventions.Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Conventions.Api.Infrastructure.AuthorizationDefinitions;

namespace Conventions.Api.Features.AttendTalk;

public class AttendTalkHandler : AsyncRequestHandler<AttendTalkCommand>
{
	private readonly ConventionsDbContext _dbContext;

	public AttendTalkHandler(ConventionsDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	protected override async Task Handle(AttendTalkCommand request, CancellationToken ctx)
	{
		var eventExists = await _dbContext.Events.AnyAsync(x => x.Id == request.EventId, ctx);
		if (!eventExists)
		{
			throw CustomHttpException.WrongEventId(request.EventId);
		}
		
		var talk = await _dbContext.Talks
			.Include(x => x.Attendees)
			.Where(x => x.EventId == request.EventId && x.Id == request.TalkId)
			.FirstOrDefaultAsync(ctx);
		if (talk is null)
		{
			throw CustomHttpException.TalkNotFound(request.TalkId, request.EventId);
		}

		var account = await _dbContext.Accounts
			.Include(x => x.TalksAsAttendee)
			.FirstOrDefaultAsync(x => x.Id == request.AccountId, ctx);
		if (account is null)
		{
			throw CustomHttpException.AttendeeNotFound(request.AccountId);
		}

		if (talk.Attendees.Any(x => x.Id == request.AccountId))
		{
			// account is already attending this talk, do nothing
			return;
		}
		
		account.TalksAsAttendee.Add(talk);
		talk.Attendees.Add(account);
		_dbContext.Update(talk);
		await _dbContext.SaveChangesAsync(ctx);
	}
}

public static class AttendTalkHttpHandler
{
	public static WebApplication MapAttendTalkHandler(this WebApplication app)
	{
		app.MapPost("/api/events/{eventId:guid}/talks/{talkId:guid}", [Authorize(AttendanceAdmin)] async (
			HttpContext context,
			[FromRoute] Guid eventId,
			[FromRoute] Guid talkId,
			[FromServices] IMediator mediator) =>
		{
			var accountId = context.GetAccountId();
			var command = new AttendTalkCommand(talkId, eventId, accountId);

			var validator = new AttendTalkCommandValidator();
			var validationResult = validator.Validate(command);
			if (!validationResult.IsValid)
			{
				throw new ValidationException(validationResult.Errors);
			}
			
			await mediator.Send(command);
			return Results.Ok();
		});
		return app;
	}
}