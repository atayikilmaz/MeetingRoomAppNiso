using System.Threading.Tasks;
using MeetingRoomApp.Models;
using MeetingRoomApp.Interfaces;

namespace MeetingRoomApp.Services
{
    public class MeetingRoomService : IMeetingRoomService
    {
        private readonly IMeetingRoomRepository _meetingRoomRepository;

        public MeetingRoomService(IMeetingRoomRepository meetingRoomRepository)
        {
            _meetingRoomRepository = meetingRoomRepository;
        }

        public async Task<MeetingRoom> CreateMeetingRoomAsync(MeetingRoom meetingRoom)
        {
            return await _meetingRoomRepository.CreateAsync(meetingRoom);
        }

        public async Task DeleteMeetingRoomAsync(int id)
        {
            await _meetingRoomRepository.DeleteAsync(id);
        }
        
        public async Task<IEnumerable<MeetingRoom>> GetAllMeetingRoomsAsync()
        {
            return await _meetingRoomRepository.GetAllAsync();
        }
    }
}