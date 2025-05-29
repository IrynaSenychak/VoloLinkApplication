using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VoloLinkApp.Areas.Identity.Data;
using VoloLinkApp.Models;

namespace VoloLinkApp.Areas.Identity.Data;

public class VoloLinkDbContext : IdentityDbContext<VoloLinkUser>
{
    public VoloLinkDbContext(DbContextOptions<VoloLinkDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }

    public DbSet<VerificationRequest> VerificationRequests { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure many-to-many relationship between users and events
        builder.Entity<Event>()
        .HasMany(e => e.Participants)
        .WithMany(u => u.ParticipatingEvents)
        .UsingEntity<Dictionary<string, object>>(
            "EventVoloLinkUser",
            j => j
                .HasOne<VoloLinkUser>()
                .WithMany()
                .HasForeignKey("ParticipantsId")
                .OnDelete(DeleteBehavior.Restrict), // не видаляти автоматично
            j => j
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey("ParticipatingEventsId")
                .OnDelete(DeleteBehavior.Cascade) // або Restrict
        );

        // Configure one-to-many relationship for event creator
        builder.Entity<Event>()
            .HasOne(e => e.Creator)
            .WithMany(u => u.CreatedEvents)
            .HasForeignKey(e => e.CreatorId);


    }
}



