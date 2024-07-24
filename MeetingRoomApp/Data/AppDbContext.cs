using MeetingRoomApp.Models;

namespace MeetingRoomApp.Data;




using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class AppDbContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public AppDbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        
        options.UseNpgsql(Environment.GetEnvironmentVariable("SUPABASE"));
    }


    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<MeetingRoom> MeetingRooms { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<MeetingParticipant> MeetingParticipants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MeetingParticipant>()
            .HasKey(mp => new { mp.MeetingId, mp.UserId });

        modelBuilder.Entity<MeetingParticipant>()
            .HasOne(mp => mp.Meeting)
            .WithMany(m => m.MeetingParticipants)
            .HasForeignKey(mp => mp.MeetingId);

        modelBuilder.Entity<MeetingParticipant>()
            .HasOne(mp => mp.User)
            .WithMany(u => u.MeetingParticipants)
            .HasForeignKey(mp => mp.UserId);
    }
}