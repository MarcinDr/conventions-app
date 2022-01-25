using MediatR;

namespace Conventions.Api.Features.GetEventsByAccount;

public record GetEventByAccountQuery(string AccountId) : IRequest<GetEventsByAccountResponse>;