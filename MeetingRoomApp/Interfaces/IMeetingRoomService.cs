using System.Threading.Tasks;
using MeetingRoomApp.Models;

namespace MeetingRoomApp.Interfaces
{
    public interface IMeetingRoomService
    {
        Task<MeetingRoom> CreateMeetingRoomAsync(MeetingRoom meetingRoom);
        Task DeleteMeetingRoomAsync(int id);
    }
}