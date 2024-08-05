using System.Threading.Tasks;
using MeetingRoomApp.Data;
using MeetingRoomApp.Models;
using MeetingRoomApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomApp.Repositories
{
    public class MeetingRoomRepository : IMeetingRoomRepository
    {
        private readonly AppDbContext _context;

        public MeetingRoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MeetingRoom> CreateAsync(MeetingRoom meetingRoom)
        {
            _context.MeetingRooms.Add(meetingRoom);
            await _context.SaveChangesAsync();
            return meetingRoom;
        }

        
        public async Task<MeetingRoom> GetByIdAsync(int id)
        {
            return await _context.MeetingRooms
                .Include(mr => mr.Meetings)
                .ThenInclude(m => m.MeetingParticipants)
                .FirstOrDefaultAsync(mr => mr.Id == id);
        }

        public async Task<IEnumerable<MeetingRoom>> GetAllAsync()
        {
            return await _context.MeetingRooms.ToListAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var meetingRoom = await _context.MeetingRooms.FindAsync(id);
            if (meetingRoom != null)
            {
                _context.MeetingRooms.Remove(meetingRoom);
                await _context.SaveChangesAsync();
            }
        }
        
    }
}