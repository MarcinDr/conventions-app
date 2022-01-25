namespace Conventions.Domain.Models;

public class Account
{
	public string Id { get; set; }
	public string? Name { get; set; }
	public string Email { get; set; }
	
	public ICollection<Talk> TalksAsAttendee { get; set; }
	public ICollection<Talk> TalksAsSpeaker { get; set; }
}