using MeetingRoomApp.Dtos;
using MeetingRoomApp.Models;

namespace MeetingRoomApp.Interfaces;

public interface IMeetingService
{
    Task<List<MeetingDto>> GetAllMeetingsAsync();
    Task<MeetingDto> GetMeetingByIdAsync(int id);
    Task<MeetingDto> CreateMeetingAsync(CreateMeetingDto createMeetingDto);
    Task<MeetingDto> UpdateMeetingAsync(UpdateMeetingDto updateMeetingDto);
    Task DeleteMeetingAsync(int id);
    
    Task<List<TimeSlot>> GetAvailableTimeSlotsAsync(int roomId, DateTime date, int? excludeMeetingId = null);

}