using Conventions.Domain;
using Conventions.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Conventions.Api.Infrastructure.AuthorizationDefinitions;

namespace Conventions.Api.Features.GetVenues;

public class GetVenuesHandler : IRequestHandler<GetVenuesQuery, IList<GetVenuesResponse>>
{
	private readonly ConventionsDbContext _dbContext;

	public GetVenuesHandler(ConventionsDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	public async Task<IList<GetVenuesResponse>> Handle(GetVenuesQuery request, CancellationToken ctx)
	{
		List<Venue> venues;
		if (string.IsNullOrWhiteSpace(request.Query))
		{
			venues = await _dbContext.Venues.ToListAsync(ctx);
		}
		else
		{
			venues = await _dbContext.Venues
				.Where(x => EF.Functions.ILike(x.Name, $"%{request.Query}%"))
				.ToListAsync(ctx);
		}

		return venues
			.Select(v => new GetVenuesResponse(v.Id, v.Name, v.Street, v.City, v.Country, v.PostalCode))
			.ToList();
	}
}

public static class GetVenuesHttpHandler
{
	public static WebApplication MapGetVenuesHandler(this WebApplication app)
	{
		app.MapGet("/api/venues", [Authorize(EventsRead)] async (
			[FromQuery] string? query,
			[FromServices] IMediator mediator) =>
		{
			var response = await mediator.Send(new GetVenuesQuery(query));
			return Results.Ok(response);
		});
		return app;
	}
}