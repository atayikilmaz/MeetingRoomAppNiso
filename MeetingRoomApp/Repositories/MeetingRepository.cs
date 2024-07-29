using MeetingRoomApp.Data;
using MeetingRoomApp.Interfaces;
using MeetingRoomApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomApp.Repositories;

public class MeetingRepository : IMeetingRepository
{
    private readonly AppDbContext _context;

    public MeetingRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Meeting>> GetAllMeetingsAsync()
    {
        var currentYear = DateTime.Now.Year;

        return await _context.Meetings
            .Where(m => m.StartDateTime.Year == currentYear)
            .Include(m => m.MeetingRoom)
            .Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.User)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Meeting>> GetUpcomingMeetingsAsync(DateTime start, DateTime end)
    {
        var meetings = await _context.Meetings
            .Where(m => m.StartDateTime >= start && m.StartDateTime <= end)
            .Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.User)
            .ToListAsync();

        return meetings;
    }
    
    public async Task<bool> IsMeetingOverlappingAsync(int roomId, DateTime start, DateTime end)
    {
        return await _context.Meetings
            .AnyAsync(m => m.MeetingRoomId == roomId && 
                           m.StartDateTime < end && 
                           m.EndDateTime > start);
    }
    
    public async Task<Meeting> GetMeetingByIdAsync(int id)
    {
        return await _context.Meetings
            .Include(m => m.MeetingRoom)
            .Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.User)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
    public async Task<Meeting> CreateMeetingAsync(Meeting meeting)
    {
        _context.Meetings.Add(meeting);
        if (meeting.MeetingParticipants != null)
        {
            _context.MeetingParticipants.AddRange(meeting.MeetingParticipants);
        }
        await _context.SaveChangesAsync();
        return meeting;
    }

    public async Task<Meeting> UpdateMeetingAsync(Meeting meeting)
    {
        _context.Entry(meeting).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return meeting;
    }

    
    public async Task DeleteMeetingAsync(int id)
    {
        var meeting = await _context.Meetings.FindAsync(id);
        if (meeting != null)
        {
            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();
        }
    }
}