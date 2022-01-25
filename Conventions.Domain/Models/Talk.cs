namespace Conventions.Domain.Models;

public class Talk
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	
	public string SpeakerId { get; set; }
	public Account Speaker { get; set; }
	
	public ICollection<Account> Attendees { get; set; }

	public Guid EventId { get; set; }
	public Event Event { get; set; }
}