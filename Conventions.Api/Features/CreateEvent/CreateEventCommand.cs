using FluentValidation;
using MediatR;

namespace Conventions.Api.Features.CreateEvent;

public record CreateEventCommand(string Name, Guid VenueId, DateTime StartDate, DateTime EndDate) 
	: IRequest<CreateEventResponse>;

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
	public CreateEventCommandValidator()
	{
		RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
		RuleFor(x => x.VenueId).NotEmpty();
		RuleFor(x => x.StartDate)
			.GreaterThan(DateTime.MinValue)
			.LessThan(x => x.EndDate).WithMessage("End date cannot be greater than start date");

		RuleFor(x => x.EndDate)
			.GreaterThan(DateTime.MinValue)
			.LessThan(DateTime.MaxValue);
	}
}