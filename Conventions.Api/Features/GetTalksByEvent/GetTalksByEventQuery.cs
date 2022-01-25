using FluentValidation;
using MediatR;

namespace Conventions.Api.Features.GetTalksByEvent;

public record GetTalksByEventQuery(Guid EventId) : IRequest<IList<GetTalksByEventResponse>>;

public class GetTalksByEventQueryValidator : AbstractValidator<GetTalksByEventQuery>
{
	public GetTalksByEventQueryValidator()
	{
		RuleFor(x => x.EventId).NotEmpty();
	}
}