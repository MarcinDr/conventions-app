#pragma warning disable CS8618
namespace Conventions.Domain.Models;

public class Event
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public Guid VenueId { get; set; }
	public Venue Venue { get; set; }
	public ICollection<Talk> Talks { get; set; }
}