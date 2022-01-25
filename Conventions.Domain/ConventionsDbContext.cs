using Conventions.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Conventions.Domain;

#pragma warning disable 8618
public class ConventionsDbContext : DbContext
{
	public DbSet<Venue> Venues { get; set; }
	public DbSet<Event> Events { get; set; }
	public DbSet<Talk> Talks { get; set; }
	public DbSet<Account> Accounts { get; set; }

	public ConventionsDbContext(DbContextOptions<ConventionsDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<Venue>(b =>
		{
			b.HasKey(x => x.Id);
			b.Property(x => x.Id)
				.ValueGeneratedOnAdd()
				.IsRequired();
		});

		builder.Entity<Event>(b =>
		{
			b.HasKey(x => x.Id);
			b.Property(x => x.Id)
				.ValueGeneratedOnAdd()
				.IsRequired();

			b.HasOne(x => x.Venue)
				.WithMany(x => x.Events)
				.HasForeignKey(x => x.VenueId)
				.IsRequired()
				.OnDelete(DeleteBehavior.NoAction);
		});
		
		builder.Entity<Talk>(b =>
		{
			b.HasKey(x => x.Id);
			b.Property(x => x.Id)
				.ValueGeneratedOnAdd()
				.IsRequired();

			b.HasOne(x => x.Event)
				.WithMany(x => x.Talks)
				.HasForeignKey(x => x.EventId)
				.IsRequired();

			b.HasOne(x => x.Speaker)
				.WithMany(x => x.TalksAsSpeaker)
				.HasForeignKey(x => x.SpeakerId)
				.IsRequired();

			b.HasMany(x => x.Attendees)
				.WithMany(x => x.TalksAsAttendee)
				.UsingEntity(j => j.ToTable("talks_attendees"));
		});
		
		builder.Entity<Account>(b =>
		{
			b.HasKey(x => x.Id);
			b.Property(x => x.Id)
				.ValueGeneratedOnAdd()
				.IsRequired();
		});
	}
}