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


    public DbSet<User> Users { get; set; }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<MeetingRoom> MeetingRooms { get; set; }


}