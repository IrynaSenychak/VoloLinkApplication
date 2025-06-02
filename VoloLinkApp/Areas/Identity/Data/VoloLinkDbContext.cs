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
        builder.Entity<Event>()
        .HasMany(e => e.Participants)
        .WithMany(u => u.ParticipatingEvents)
        .UsingEntity<Dictionary<string, object>>(
            "EventVoloLinkUser",
            j => j
                .HasOne<VoloLinkUser>()
                .WithMany()
                .HasForeignKey("ParticipantsId")
                .OnDelete(DeleteBehavior.Restrict), 
            j => j
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey("ParticipatingEventsId")
                .OnDelete(DeleteBehavior.Cascade) 
        );
        builder.Entity<Event>()
        .HasMany(e => e.AttendedParticipants)
        .WithMany()
        .UsingEntity<Dictionary<string, object>>(
            "EventAttendance",
            j => j
                .HasOne<VoloLinkUser>()
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Restrict),
            j => j
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey("EventId")
                .OnDelete(DeleteBehavior.Cascade)
        );

        builder.Entity<Event>()
            .HasOne(e => e.Creator)
            .WithMany(u => u.CreatedEvents)
            .HasForeignKey(e => e.CreatorId);
    }
}



