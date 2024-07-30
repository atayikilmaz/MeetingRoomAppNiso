using System.Collections.Immutable;
using MeetingRoomApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MeetingRoomApp.Data
{
    public class AppDbContext : IdentityDbContext
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MeetingParticipant>()
                .HasOne(mp => mp.User)
                .WithMany(u => u.MeetingParticipants)
                .HasForeignKey(mp => mp.ParticipantId)
                .IsRequired();
            
            modelBuilder.Entity<MeetingRoom>()
                .HasIndex(m => m.Name)
                .IsUnique();
        }

        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MeetingRoom> MeetingRooms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MeetingParticipant> MeetingParticipants { get; set; }
    }
}