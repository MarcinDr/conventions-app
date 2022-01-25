using FluentValidation;
using MediatR;

namespace Conventions.Api.Features.CreateTalk;

public record CreateTalkCommand(string Name, DateTime StartDate, DateTime EndDate, string SpeakerId)
	: IRequest<CreateTalkResponse>
{
	public Guid EventId { get; set; }
}



public class CreateTalkCommandValidator : AbstractValidator<CreateTalkCommand>
{
	public CreateTalkCommandValidator()
	{
		RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
		RuleFor(x => x.SpeakerId).NotEmpty();
		// RuleFor(x => x.EventId).NotEmpty();
		RuleFor(x => x.StartDate)
			.GreaterThan(DateTime.MinValue)
			.LessThan(x => x.EndDate).WithMessage("End date cannot be greater than start date");

		RuleFor(x => x.EndDate)
			.GreaterThan(DateTime.MinValue)
			.LessThan(DateTime.MaxValue);
	}
}