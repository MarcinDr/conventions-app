using FluentValidation;
using MediatR;

namespace Conventions.Api.Features.GetTalkById;

public record GetTalkByIdQuery(Guid Id, Guid EventId) : IRequest<GetTalkByIdResponse>;

public class GetTalkByIdQueryValidator : AbstractValidator<GetTalkByIdQuery>
{
	public GetTalkByIdQueryValidator()
	{
		RuleFor(x => x.Id).NotEmpty();
		RuleFor(x => x.EventId).NotEmpty();
	}
}