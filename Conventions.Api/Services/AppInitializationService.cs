using Bogus;
using Conventions.Domain;
using Conventions.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Conventions.Api.Services;

public class AppInitializationService
{
	private readonly IBreweriesApi _breweriesApi;
	private readonly ConventionsDbContext _dbContext;

	public AppInitializationService(
		IBreweriesApi breweriesApi,
		ConventionsDbContext dbContext)
	{
		Randomizer.Seed = new Random(8675309);
		_breweriesApi = breweriesApi;
		_dbContext = dbContext;
	}

	public async Task Initialize()
	{
		await _dbContext.Database.MigrateAsync();
		
		// db already seeded nothing to do
		var venuesExist = await _dbContext.Venues.AnyAsync();
		if (venuesExist)
		{
			return;
		}

		await using var transaction = await _dbContext.Database.BeginTransactionAsync();

		var users = await CreateAccounts();
		var venues = await CreateVenues();

		await CreateEventsWithTalks(users, venues);

		await _dbContext.SaveChangesAsync();
		await transaction.CommitAsync();
	}

	private async Task CreateEventsWithTalks(IList<Account> users, IList<Venue> venues)
	{
		foreach (var venue in venues)
		{
			var events = new Faker<Event>()
				.RuleFor(x => x.Id, f => f.Random.Guid())
				.RuleFor(x => x.Name, f => $"{f.Company.CompanyName()} - {f.Commerce.Color()}")
				.RuleFor(x => x.StartDate, f => f.Date.Recent(Randomizer.Seed.Next(1, 50)).ToUniversalTime())
				.RuleFor(x => x.EndDate, f => f.Date.Soon(Randomizer.Seed.Next(1, 50)).ToUniversalTime())
				.RuleFor(x => x.VenueId, venue.Id)
				.GenerateBetween(1, 5);

			foreach (var @event in events)
			{
				// this will generate talks where speaker might be an admin or attendee but...for simplicity we will allow it
				@event.Talks = new Faker<Talk>()
					.RuleFor(x => x.Name, f => $"{f.Vehicle.Model()} - {f.Hacker.Verb()}")
					.RuleFor(x => x.SpeakerId, f => f.Random.CollectionItem(users).Id)
					.RuleFor(x => x.StartDate, f => f.Date.Between(@event.StartDate, @event.EndDate.AddMinutes(-10).ToUniversalTime()))
					.RuleFor(x => x.EndDate, (f, c) => f.Date.Between(c.StartDate, @event.EndDate).ToUniversalTime())
					.RuleFor(x => x.EventId, @event.Id)
					.GenerateBetween(3, 10);
			}

			_dbContext.AddRange(events);
		}

		await _dbContext.SaveChangesAsync();
	}

	private async Task<IList<Account>> CreateAccounts()
	{
		var admin = new Account
		{
			Id = "auth0|61e2ee8552426b006831fc36",
			Email = "druzgala.marcin+admin@gmail.com",
			Name = "Administrator",
		};
		
		var attendee = new Account
		{
			Id = "auth0|61e2eecd8006ea006a0942dc",
			Email = "druzgala.marcin+attendee@gmail.com",
			Name = "Attendee",
		};
		
		var speaker = new Account
		{
			Id = "auth0|61e2eee9c937c4006a7c4b56",
			Email = "druzgala.marcin+speaker@gmail.com",
			Name = "Speaker",
		};
		
		_dbContext.AddRange(admin, attendee, speaker);
		await _dbContext.SaveChangesAsync();

		return new List<Account> { admin, attendee, speaker };
	}

	private async Task<IList<Venue>> CreateVenues()
	{
		var breweries = await _breweriesApi.GetAll();
		var venues = breweries.Select(b => new Venue
		{
			Id = Guid.NewGuid(),
			City = b.City,
			Country = b.Country,
			Name = b.Name,
			Phone = b.Phone,
			Street = b.Street,
			CreatedAt = b.CreatedAt,
			ExternalId = b.Id,
			PostalCode = b.PostalCode,
			UpdatedAt = b.UpdatedAt,
			WebsiteUrl = b.WebsiteUrl,
		}).ToList();
	
		_dbContext.AddRange(venues);
		await _dbContext.SaveChangesAsync();
		return venues;
	}
}