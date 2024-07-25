using System.Threading.Tasks;
using MeetingRoomApp.Models;

namespace MeetingRoomApp.Interfaces
{
    public interface IMeetingRoomRepository
    {
        Task<MeetingRoom> CreateAsync(MeetingRoom meetingRoom);
        Task DeleteAsync(int id);
        
        Task<IEnumerable<MeetingRoom>> GetAllAsync();

    }
}