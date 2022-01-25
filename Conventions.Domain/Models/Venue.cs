namespace Conventions.Domain.Models;

public class Venue
{
	public Guid Id { get; set; }
	public string? ExternalId { get; set; }
	public string Name { get; set; }
	public string Street { get; set; }
	public string City { get; set; }
	public string PostalCode { get; set; }
	public string Country { get; set; }
	public string Phone { get; set; }
	public string? WebsiteUrl { get; set; }
	public DateTime UpdatedAt { get; set; }
	public DateTime CreatedAt { get; set; }
	
	public ICollection<Event> Events { get; set; }
}