using FluentValidation;
using MediatR;

namespace Conventions.Api.Features.AttendTalk;

public record AttendTalkCommand(Guid TalkId, Guid EventId, string AccountId) : IRequest;

public class AttendTalkCommandValidator : AbstractValidator<AttendTalkCommand>
{
	public AttendTalkCommandValidator()
	{
		RuleFor(x => x.AccountId).NotEmpty();
		RuleFor(x => x.EventId).NotEmpty();
		RuleFor(x => x.AccountId).NotEmpty();
	}
}