using MeetingRoomApp.Models;

namespace MeetingRoomApp.Interfaces;

public interface IMeetingRepository
{
    Task<List<Meeting>> GetAllMeetingsAsync();
    Task<Meeting> GetMeetingByIdAsync(int id);
    Task<Meeting> CreateMeetingAsync(Meeting meeting);
    Task<Meeting> UpdateMeetingAsync(Meeting meeting);
    Task DeleteMeetingAsync(int id);
    
    Task<bool> IsMeetingOverlappingAsync(int roomId, DateTime start, DateTime end);

    Task<IEnumerable<Meeting>> GetUpcomingMeetingsAsync(DateTime start, DateTime end);
}